using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeanRaw.Web.Data;

namespace SeanRaw.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        // ── GET / ── Trang chủ ───────────────────────────────
        public async Task<IActionResult> Index()
        {
            ViewBag.GoiDichVu   = await _db.ServicePackages.ToListAsync();
            ViewBag.SanPham     = await _db.Products
                                    .Where(p => !p.IsDeleted)
                                    .Take(4)
                                    .ToListAsync();
            ViewBag.UserName    = HttpContext.Session.GetString("UserName");
            return View();
        }

        // ── GET /Home/Store ── Cửa hàng phụ kiện ───────────
        public async Task<IActionResult> Store()
        {
            var products = await _db.Products
                .Where(p => !p.IsDeleted)
                .ToListAsync();
            return View(products);
        }
    }

    // ─────────────────────────────────────────────────────────
    // Admin Controller
    // ─────────────────────────────────────────────────────────
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        private bool IsAdmin() =>
            HttpContext.Session.GetString("UserRole") == "admin";

        // ── GET /Admin ── Dashboard tổng quan ───────────────
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ViewBag.TongNguoiDung   = await _db.Users.CountAsync();
            ViewBag.TongBooking     = await _db.Bookings.CountAsync();
            ViewBag.BookingChoXuLy  = await _db.Bookings
                                        .CountAsync(b => b.TrangThaiBooking == "cho_dat_coc");
            ViewBag.ThoChoXacThuc   = await _db.PhotographerProfiles
                                        .CountAsync(p => p.TrangThaiXacThuc == "cho_duyet");

            var bookings = await _db.Bookings
                .Include(b => b.KhachHang)
                .Include(b => b.Photographer)
                .Include(b => b.ServicePackage)
                .OrderByDescending(b => b.IdBooking)
                .Take(10)
                .ToListAsync();

            return View(bookings);
        }

        // ── GET /Admin/Photographers ── Quản lý thợ ảnh ─────
        public async Task<IActionResult> Photographers()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var list = await _db.PhotographerProfiles
                .Include(p => p.User)
                .OrderBy(p => p.TrangThaiXacThuc)
                .ToListAsync();

            return View(list);
        }

        // ── POST /Admin/ApprovePhotographer ─────────────────
        [HttpPost]
        public async Task<IActionResult> ApprovePhotographer(int id, string action)
        {
            if (!IsAdmin()) return Forbid();

            var profile = await _db.PhotographerProfiles.FindAsync(id);
            if (profile == null) return NotFound();

            profile.TrangThaiXacThuc = action == "approve" ? "da_xac_thuc" : "tu_choi";

            // Ghi audit log
            _db.AuditLogs.Add(new Models.AuditLog
            {
                IdNguoiDung         = HttpContext.Session.GetString("UserId")!,
                HanhVi              = action == "approve" ? "APPROVE_PHOTOGRAPHER" : "REJECT_PHOTOGRAPHER",
                DoiTuongBiTacDong   = "photographer_profiles",
                IdDoiTuong          = id.ToString(),
                DiaChiIp            = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"
            });

            await _db.SaveChangesAsync();
            TempData["Success"] = action == "approve" ? "Đã duyệt thợ ảnh!" : "Đã từ chối.";
            return RedirectToAction("Photographers");
        }

        // ── GET /Admin/Users ── Quản lý người dùng ──────────
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var users = await _db.Users
                .OrderByDescending(u => u.NgayTaoTaiKhoan)
                .ToListAsync();

            return View(users);
        }
    }
}
