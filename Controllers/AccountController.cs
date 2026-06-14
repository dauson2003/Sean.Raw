using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeanRaw.Web.Data;
using SeanRaw.Web.Models;

namespace SeanRaw.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db)
        {
            _db = db;
        }

        // ── GET /Account/Login ───────────────────────────────
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserId") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // ── POST /Account/Login ──────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.DiaChiEmail == model.Email && u.TrangThaiTaiKhoan == 1);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.MatKhau, user.MatKhauBam))
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View(model);
            }

            // Lưu session
            HttpContext.Session.SetString("UserId",     user.IdNguoiDung);
            HttpContext.Session.SetString("UserName",   user.HoTenDayDu);
            HttpContext.Session.SetString("UserRole",   user.VaiTroNguoiDung);

            // Ghi audit log
            _db.AuditLogs.Add(new AuditLog
            {
                IdNguoiDung     = user.IdNguoiDung,
                HanhVi          = "LOGIN_SUCCESS",
                DiaChiIp        = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                ThietBiDauCuoi  = Request.Headers["User-Agent"].ToString()
            });
            await _db.SaveChangesAsync();

            // Điều hướng theo vai trò
            return user.VaiTroNguoiDung switch
            {
                "admin"         => RedirectToAction("Index", "Admin"),
                "photographer"  => RedirectToAction("Dashboard", "Photographer"),
                _               => RedirectToAction("Index", "Home")
            };
        }

        // ── GET /Account/Logout ──────────────────────────────
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ── GET /Account/Register ────────────────────────────
        [HttpGet]
        public IActionResult Register() => View();

        // ── POST /Account/Register ───────────────────────────
        [HttpPost]
        public async Task<IActionResult> Register(User model, string matKhau)
        {
            // Kiểm tra email trùng
            if (await _db.Users.AnyAsync(u => u.DiaChiEmail == model.DiaChiEmail))
            {
                ModelState.AddModelError("DiaChiEmail", "Email đã được sử dụng.");
                return View(model);
            }

            model.IdNguoiDung   = Guid.NewGuid().ToString();
            model.MatKhauBam    = BCrypt.Net.BCrypt.HashPassword(matKhau);
            model.VaiTroNguoiDung = "client";
            model.NgayTaoTaiKhoan = DateTime.UtcNow;

            _db.Users.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }
    }
}
