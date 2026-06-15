using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeanRaw.Web.Data;
using SeanRaw.Web.Models;

namespace SeanRaw.Web.Controllers
{
    public class PhotographerController : Controller
    {
        private readonly AppDbContext _db;

        public PhotographerController(AppDbContext db)
        {
            _db = db;
        }

        // ── Helper: Kiểm tra đã đăng nhập ────────────────────
        private string? CurrentUserId => HttpContext.Session.GetString("UserId");

        private bool IsPhotographer() =>
            HttpContext.Session.GetString("UserRole") == "photographer";

        // ── GET /Photographer/Dashboard ── Dashboard thợ ảnh ──
        public async Task<IActionResult> Dashboard()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsPhotographer())
                return RedirectToAction("Index", "Home");

            // Thống kê
            var totalBookings = await _db.Bookings
                .CountAsync(b => b.IdPhotographer == CurrentUserId);

            var pendingBookings = await _db.Bookings
                .CountAsync(b => b.IdPhotographer == CurrentUserId && b.TrangThaiBooking == "cho_dat_coc");

            var completedBookings = await _db.Bookings
                .CountAsync(b => b.IdPhotographer == CurrentUserId && b.TrangThaiBooking == "hoan_tat");

            var totalRevenue = await _db.Bookings
                .Where(b => b.IdPhotographer == CurrentUserId && b.TrangThaiBooking == "hoan_tat")
                .SumAsync(b => b.TongTienHopDong);

            ViewBag.TotalBookings = totalBookings;
            ViewBag.PendingBookings = pendingBookings;
            ViewBag.CompletedBookings = completedBookings;
            ViewBag.TotalRevenue = totalRevenue;

            // 10 booking gần đây
            var recentBookings = await _db.Bookings
                .Where(b => b.IdPhotographer == CurrentUserId)
                .Include(b => b.KhachHang)
                .Include(b => b.ServicePackage)
                .OrderByDescending(b => b.IdBooking)
                .Take(10)
                .ToListAsync();

            return View(recentBookings);
        }

        // ── GET /Photographer/Schedule ── Lịch trống & Booking ──
        public async Task<IActionResult> Schedule()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsPhotographer())
                return RedirectToAction("Index", "Home");

            var bookings = await _db.Bookings
                .Where(b => b.IdPhotographer == CurrentUserId)
                .Include(b => b.KhachHang)
                .Include(b => b.ServicePackage)
                .OrderBy(b => b.NgayChupThucTe)
                .ToListAsync();

            return View(bookings);
        }

        // ── GET /Photographer/Profile ── Hồ sơ thợ ảnh ────────
        public async Task<IActionResult> Profile()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsPhotographer())
                return RedirectToAction("Index", "Home");

            var user = await _db.Users.FindAsync(CurrentUserId);
            var profile = await _db.PhotographerProfiles
                .FirstOrDefaultAsync(p => p.IdNguoiDung == CurrentUserId);

            ViewBag.Profile = profile;
            return View(user);
        }

        // ── POST /Photographer/Profile ── Cập nhật hồ sơ ────
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User model, string? gioiThieu)
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsPhotographer())
                return RedirectToAction("Index", "Home");

            var user = await _db.Users.FindAsync(CurrentUserId);
            if (user == null) return NotFound();

            user.HoTenDayDu = model.HoTenDayDu;
            user.SoDienThoai = model.SoDienThoai;

            var profile = await _db.PhotographerProfiles
                .FirstOrDefaultAsync(p => p.IdNguoiDung == CurrentUserId);

            if (profile != null && gioiThieu != null)
            {
                profile.GioiThieuPhongCach = gioiThieu;
            }

            _db.Users.Update(user);
            if (profile != null) _db.PhotographerProfiles.Update(profile);

            await _db.SaveChangesAsync();

            TempData["Success"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction("Profile");
        }

        // ── GET /Photographer/Albums ── Quản lý album ────────
        public async Task<IActionResult> Albums()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsPhotographer())
                return RedirectToAction("Index", "Home");

            var albums = await _db.Albums
                .Include(a => a.Booking)
                    .ThenInclude(b => b.KhachHang)
                .Where(a => a.Booking.IdPhotographer == CurrentUserId)
                .OrderByDescending(a => a.IdAlbum)
                .ToListAsync();

            return View(albums);
        }

        // ── GET /Photographer/Earnings ── Thống kê doanh thu ──
        public async Task<IActionResult> Earnings()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsPhotographer())
                return RedirectToAction("Index", "Home");

            var bookings = await _db.Bookings
                .Where(b => b.IdPhotographer == CurrentUserId)
                .OrderByDescending(b => b.NgayChupThucTe)
                .ToListAsync();

            var monthlyEarnings = bookings
                .Where(b => b.TrangThaiBooking == "hoan_tat")
                .GroupBy(b => new { b.NgayChupThucTe.Year, b.NgayChupThucTe.Month })
                .Select(g => new
                {
                    Month = $"{g.Key.Month}/{g.Key.Year}",
                    Revenue = g.Sum(b => b.TongTienHopDong)
                })
                .ToList();

            ViewBag.MonthlyEarnings = monthlyEarnings;
            return View(bookings);
        }

        // ── GET /Photographer/Reviews ── Đánh giá từ khách ────
        public async Task<IActionResult> Reviews()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            if (!IsPhotographer())
                return RedirectToAction("Index", "Home");

            var reviews = await _db.Reviews
                .Include(r => r.Booking)
                    .ThenInclude(b => b.KhachHang)
                .Where(r => r.Booking.IdPhotographer == CurrentUserId)
                .OrderByDescending(r => r.IdDanhGia)
                .ToListAsync();

            var avgRating = reviews.Any()
                ? Math.Round(reviews.Average(r => r.SoSaoChatLuong), 1)
                : 5.0;

            ViewBag.AverageRating = avgRating;
            return View(reviews);
        }
    }
}
