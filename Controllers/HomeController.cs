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
            ViewBag.GoiDichVu = await _db.ServicePackages.ToListAsync();
            ViewBag.SanPham = await _db.Products
                                    .Where(p => !p.IsDeleted)
                                    .OrderBy(p => p.IdSanPham)
                                    .Take(4)
                                    .ToListAsync();
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        // ── GET /Home/Store ── Cửa hàng phụ kiện ───────────
        public async Task<IActionResult> Store()
        {
            var products = await _db.Products
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.IdSanPham)
                .ToListAsync();

            ViewBag.SanPham = products;
            return View();
        }

        // ── GET /Home/PhotographerProfile/{id} ── Hồ sơ công khai thợ ảnh ──
        public async Task<IActionResult> PhotographerProfile(string id)
        {
            var photographer = await _db.Users
                .Include(u => u.PhotographerProfile)
                .FirstOrDefaultAsync(u => u.IdNguoiDung == id && u.VaiTroNguoiDung == "photographer");

            if (photographer == null) return NotFound();

            ViewBag.GoiDichVu = await _db.ServicePackages.ToListAsync();

            var reviews = await _db.Reviews
                .Include(r => r.Booking)
                    .ThenInclude(b => b.KhachHang)
                .Where(r => r.Booking.IdPhotographer == id)
                .OrderByDescending(r => r.IdDanhGia)
                .Take(10)
                .ToListAsync();

            ViewBag.Reviews = reviews;

            var completedCount = await _db.Bookings
                .CountAsync(b => b.IdPhotographer == id && b.TrangThaiBooking == "hoan_tat");
            ViewBag.CompletedCount = completedCount;

            return View(photographer);
        }

        // ── GET /Home/FixPasswords ── Fix test user passwords ─
        // Tạm thời dùng để hash mật khẩu các user test
        // Chỉ chạy lần đầu, sau đó xóa endpoint này
        [HttpGet]
        public async Task<IActionResult> FixPasswords()
        {
            if (!HttpContext.Request.Host.Host.Contains("localhost") &&
                !HttpContext.Request.Host.Host.Contains("127.0.0.1"))
            {
                return BadRequest("Chỉ chạy trên localhost");
            }

            var users = await _db.Users.ToListAsync();
            int fixedCount = 0;

            foreach (var user in users)
            {
                // Kiểm tra nếu password chưa được hash BCrypt (không bắt đầu với $2)
                if (!user.MatKhauBam.StartsWith("$2"))
                {
                    // Hash lại mật khẩu (giả sử mật khẩu hiện tại đó là plaintext)
                    // Mật khẩu mặc định là "123456" cho test
                    user.MatKhauBam = BCrypt.Net.BCrypt.HashPassword("123456");
                    _db.Users.Update(user);
                    fixedCount++;
                }
            }

            if (fixedCount > 0)
            {
                await _db.SaveChangesAsync();
            }

            return Ok($"Đã fix {fixedCount} user(s). Bây giờ có thể đăng nhập với mật khẩu: 123456");
        }
    }
}
