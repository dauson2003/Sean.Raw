using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeanRaw.Web.Data;
using SeanRaw.Web.Models;

namespace SeanRaw.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _db;

        public CustomerController(AppDbContext db)
        {
            _db = db;
        }

        // ── Helper: Kiểm tra đã đăng nhập ────────────────────
        private string? CurrentUserId => HttpContext.Session.GetString("UserId");

        private bool IsCustomer() =>
            HttpContext.Session.GetString("UserRole") == "client";

        // ── GET /Customer/Dashboard ── Dashboard khách hàng ──
        public async Task<IActionResult> Dashboard()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsCustomer())
                return RedirectToAction("Index", "Home");

            // Thống kê
            var totalBookings = await _db.Bookings
                .CountAsync(b => b.IdKhachHang == CurrentUserId);

            var pendingBookings = await _db.Bookings
                .CountAsync(b => b.IdKhachHang == CurrentUserId && b.TrangThaiBooking == "cho_dat_coc");

            var completedBookings = await _db.Bookings
                .CountAsync(b => b.IdKhachHang == CurrentUserId && b.TrangThaiBooking == "hoan_tat");

            var totalSpent = await _db.Bookings
                .Where(b => b.IdKhachHang == CurrentUserId)
                .SumAsync(b => b.TongTienHopDong);

            ViewBag.TotalBookings = totalBookings;
            ViewBag.PendingBookings = pendingBookings;
            ViewBag.CompletedBookings = completedBookings;
            ViewBag.TotalSpent = totalSpent;

            // 6 booking gần đây
            var recentBookings = await _db.Bookings
                .Where(b => b.IdKhachHang == CurrentUserId)
                .Include(b => b.Photographer)
                .Include(b => b.ServicePackage)
                .OrderByDescending(b => b.IdBooking)
                .Take(6)
                .ToListAsync();

            return View(recentBookings);
        }

        // ── GET /Customer/Profile ── Hồ sơ cá nhân ──────────
        public async Task<IActionResult> Profile()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsCustomer())
                return RedirectToAction("Index", "Home");

            var user = await _db.Users.FindAsync(CurrentUserId);
            return View(user);
        }

        // ── POST /Customer/Profile ── Cập nhật hồ sơ ────────
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User model)
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsCustomer())
                return RedirectToAction("Index", "Home");

            var user = await _db.Users.FindAsync(CurrentUserId);
            if (user == null) return NotFound();

            user.HoTenDayDu = model.HoTenDayDu;
            user.SoDienThoai = model.SoDienThoai;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction("Profile");
        }

        // ── GET /Customer/Bookings ── Lịch sử đặt lịch ──────
        public async Task<IActionResult> Bookings()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsCustomer())
                return RedirectToAction("Index", "Home");

            var bookings = await _db.Bookings
                .Where(b => b.IdKhachHang == CurrentUserId)
                .Include(b => b.Photographer)
                .Include(b => b.ServicePackage)
                .OrderByDescending(b => b.NgayChupThucTe)
                .ToListAsync();

            return View(bookings);
        }

        // ── GET /Customer/Albums ── Thư viện ảnh của tôi ────
        public async Task<IActionResult> Albums()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsCustomer())
                return RedirectToAction("Index", "Home");

            var albums = await _db.Albums
                .Include(a => a.Booking)
                    .ThenInclude(b => b.Photographer)
                .Where(a => a.Booking.IdKhachHang == CurrentUserId)
                .OrderByDescending(a => a.IdAlbum)
                .ToListAsync();

            return View(albums);
        }

        // ── GET /Customer/Orders ── Lịch sử mua sắm ────────
        public async Task<IActionResult> Orders()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsCustomer())
                return RedirectToAction("Index", "Home");

            var orders = await _db.StoreOrders
                .Where(o => o.IdKhachHang == CurrentUserId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.IdDonHang)
                .ToListAsync();

            return View(orders);
        }

        // ── GET /Customer/Favorites ── Sản phẩm yêu thích ───
        public async Task<IActionResult> Favorites()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsCustomer())
                return RedirectToAction("Index", "Home");

            // TODO: Nếu có bảng Favorites, query ở đây
            var products = await _db.Products
                .Where(p => !p.IsDeleted)
                .ToListAsync();

            return View(products);
        }

        // ── GET /Customer/Reviews ── Đánh giá của tôi ──────
        public async Task<IActionResult> Reviews()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsCustomer())
                return RedirectToAction("Index", "Home");

            var reviews = await _db.Reviews
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Photographer)
                .Where(r => r.Booking.IdKhachHang == CurrentUserId)
                .OrderByDescending(r => r.IdDanhGia)
                .ToListAsync();

            return View(reviews);
        }

        // ── POST /Customer/WriteReview ── Viết đánh giá ────
        [HttpPost]
        public async Task<IActionResult> WriteReview(int bookingId, int rating, string comment)
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsCustomer())
                return RedirectToAction("Index", "Home");

            var booking = await _db.Bookings.FindAsync(bookingId);
            if (booking == null || booking.IdKhachHang != CurrentUserId)
                return BadRequest("Booking không tồn tại hoặc không thuộc về bạn.");

            var existingReview = await _db.Reviews
                .FirstOrDefaultAsync(r => r.IdBooking == bookingId);

            if (existingReview != null)
            {
                existingReview.SoSaoChatLuong = (short)rating;
                existingReview.NoiDungNhanXet = comment;
                existingReview.NgayTaoPhanHoi = DateTime.UtcNow;
                _db.Reviews.Update(existingReview);
            }
            else
            {
                var review = new Review
                {
                    IdBooking = bookingId,
                    SoSaoChatLuong = (short)rating,
                    NoiDungNhanXet = comment,
                    NgayTaoPhanHoi = DateTime.UtcNow
                };
                _db.Reviews.Add(review);
            }

            await _db.SaveChangesAsync();

            TempData["Success"] = "Đánh giá thành công!";
            return RedirectToAction("Reviews");
        }
    }
}
