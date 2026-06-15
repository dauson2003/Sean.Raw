using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeanRaw.Web.Data;
using SeanRaw.Web.Models;

namespace SeanRaw.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _db;

        public BookingController(AppDbContext db)
        {
            _db = db;
        }

        // Helper: kiểm tra đăng nhập
        private string? CurrentUserId => HttpContext.Session.GetString("UserId");

        // ── GET /Booking ─── Danh sách booking của khách ────
        public async Task<IActionResult> Index()
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            var bookings = await _db.Bookings
                .Include(b => b.ServicePackage)
                .Include(b => b.Photographer)
                .Where(b => b.IdKhachHang == CurrentUserId)
                .OrderByDescending(b => b.NgayChupThucTe)
                .ToListAsync();

            return View(bookings);
        }

        // ── GET /Booking/Create ── Form đặt lịch mới ────────
        public async Task<IActionResult> Create()
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            ViewBag.GoiDichVu = await _db.ServicePackages.ToListAsync();
            ViewBag.ThoPhanh  = await _db.Users
                .Join(_db.PhotographerProfiles,
                    u => u.IdNguoiDung,
                    p => p.IdNguoiDung,
                    (u, p) => new { u, p })
                .Where(x => x.p.TrangThaiXacThuc == "da_xac_thuc")
                .Select(x => x.u)
                .ToListAsync();

            return View();
        }

        // ── POST /Booking/Create ─────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Create(BookingViewModel model)
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            var goi = await _db.ServicePackages.FindAsync(model.IdGoiDichVu);
            if (goi == null) return BadRequest();

            var booking = new Booking
            {
                IdKhachHang     = CurrentUserId,
                IdPhotographer  = model.IdPhotographer,
                IdGoiDichVu     = model.IdGoiDichVu,
                NgayChupThucTe  = model.NgayChup,
                KhungGioChup    = model.KhungGio,
                DiaDiemChup     = model.DiaDiem,
                TongTienHopDong = goi.GiaTien,
                TrangThaiBooking = "cho_dat_coc"
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Đặt lịch thành công! Vui lòng thanh toán cọc để xác nhận.";
            return RedirectToAction("Confirmation", new { id = booking.IdBooking });
        }

        // ── GET /Booking/Confirmation/5 ── Trang xác nhận sau khi đặt ──
        public async Task<IActionResult> Confirmation(int id)
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            var booking = await _db.Bookings
                .Include(b => b.ServicePackage)
                .Include(b => b.Photographer)
                .FirstOrDefaultAsync(b => b.IdBooking == id && b.IdKhachHang == CurrentUserId);

            if (booking == null) return NotFound();
            return View(booking);
        }

        // ── GET /Booking/Detail/5 ────────────────────────────
        public async Task<IActionResult> Detail(int id)
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            var booking = await _db.Bookings
                .Include(b => b.ServicePackage)
                .Include(b => b.Photographer)
                .Include(b => b.Album)
                .Include(b => b.Review)
                .FirstOrDefaultAsync(b => b.IdBooking == id && b.IdKhachHang == CurrentUserId);

            if (booking == null) return NotFound();
            return View(booking);
        }
    }
}
