using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeanRaw.Web.Data;
using SeanRaw.Web.Models;

namespace SeanRaw.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _db;

        public CartController(AppDbContext db)
        {
            _db = db;
        }

        // ── Helper: kiểm tra đăng nhập + vai trò khách hàng ──
        private string? CurrentUserId => HttpContext.Session.GetString("UserId");

        private bool IsCustomer() =>
            HttpContext.Session.GetString("UserRole") == "client";

        // Trạng thái "trong_gio" = đơn hàng nháp, chưa đặt thật.
        // Khi checkout thành công, chuyển sang "cho_thanh_toan" (đơn hàng thật).
        private const string TRANG_THAI_GIO_HANG = "trong_gio";

        // ── Lấy (hoặc tạo) giỏ hàng hiện tại của khách ───────
        private async Task<StoreOrder> GetOrCreateCartAsync(string userId)
        {
            var cart = await _db.StoreOrders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.IdKhachHang == userId && o.TrangThaiThanhToan == TRANG_THAI_GIO_HANG);

            if (cart == null)
            {
                cart = new StoreOrder
                {
                    IdKhachHang = userId,
                    NgayDatHang = DateTime.UtcNow,
                    TongTienHoaDon = 0,
                    TrangThaiThanhToan = TRANG_THAI_GIO_HANG,
                    DiaChiGiaoHang = ""
                };
                _db.StoreOrders.Add(cart);
                await _db.SaveChangesAsync();
            }

            return cart;
        }

        private async Task RecalculateTotalAsync(StoreOrder cart)
        {
            await _db.Entry(cart).Collection(o => o.OrderItems).LoadAsync();
            cart.TongTienHoaDon = cart.OrderItems.Sum(oi => oi.GiaGiaoDich * oi.SoLuongMua);
            _db.StoreOrders.Update(cart);
            await _db.SaveChangesAsync();
        }

        // ── GET /Cart ── Xem giỏ hàng ─────────────────────────
        public async Task<IActionResult> Index()
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");
            if (!IsCustomer()) return RedirectToAction("Index", "Home");

            var cart = await GetOrCreateCartAsync(CurrentUserId);
            return View(cart);
        }

        // ── POST /Cart/Add ── Thêm sản phẩm vào giỏ ──────────
        [HttpPost]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");
            if (!IsCustomer())
            {
                TempData["Error"] = "Chỉ khách hàng mới có thể mua sản phẩm.";
                return RedirectToAction("Store", "Home");
            }

            var product = await _db.Products.FindAsync(productId);
            if (product == null || product.IsDeleted)
                return NotFound();

            if (quantity < 1) quantity = 1;
            if (product.SoLuongTonKho < quantity)
            {
                TempData["Error"] = "Sản phẩm không đủ số lượng trong kho.";
                return RedirectToAction("Store", "Home");
            }

            var cart = await GetOrCreateCartAsync(CurrentUserId);

            var existingItem = cart.OrderItems.FirstOrDefault(oi => oi.IdSanPham == productId);
            if (existingItem != null)
            {
                var newQty = existingItem.SoLuongMua + quantity;
                if (newQty > product.SoLuongTonKho) newQty = product.SoLuongTonKho;
                existingItem.SoLuongMua = newQty;
                existingItem.GiaGiaoDich = product.GiaBanNiemYet;
                _db.OrderItems.Update(existingItem);
            }
            else
            {
                _db.OrderItems.Add(new OrderItem
                {
                    IdDonHang = cart.IdDonHang,
                    IdSanPham = productId,
                    SoLuongMua = quantity,
                    GiaGiaoDich = product.GiaBanNiemYet
                });
            }

            await _db.SaveChangesAsync();
            await RecalculateTotalAsync(cart);

            TempData["Success"] = $"Đã thêm \"{product.TenSanPham}\" vào giỏ hàng!";
            return RedirectToAction("Store", "Home");
        }

        // ── POST /Cart/UpdateQuantity ── Cập nhật số lượng ────
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int itemId, int quantity)
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            var item = await _db.OrderItems
                .Include(oi => oi.StoreOrder)
                .Include(oi => oi.Product)
                .FirstOrDefaultAsync(oi => oi.IdChiTiet == itemId);

            if (item == null || item.StoreOrder.IdKhachHang != CurrentUserId
                || item.StoreOrder.TrangThaiThanhToan != TRANG_THAI_GIO_HANG)
                return NotFound();

            if (quantity <= 0)
            {
                _db.OrderItems.Remove(item);
            }
            else
            {
                if (quantity > item.Product.SoLuongTonKho) quantity = item.Product.SoLuongTonKho;
                item.SoLuongMua = quantity;
                _db.OrderItems.Update(item);
            }

            await _db.SaveChangesAsync();
            await RecalculateTotalAsync(item.StoreOrder);

            return RedirectToAction("Index");
        }

        // ── POST /Cart/Remove ── Xóa sản phẩm khỏi giỏ ────────
        [HttpPost]
        public async Task<IActionResult> Remove(int itemId)
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            var item = await _db.OrderItems
                .Include(oi => oi.StoreOrder)
                .FirstOrDefaultAsync(oi => oi.IdChiTiet == itemId);

            if (item == null || item.StoreOrder.IdKhachHang != CurrentUserId
                || item.StoreOrder.TrangThaiThanhToan != TRANG_THAI_GIO_HANG)
                return NotFound();

            var cart = item.StoreOrder;
            _db.OrderItems.Remove(item);
            await _db.SaveChangesAsync();
            await RecalculateTotalAsync(cart);

            return RedirectToAction("Index");
        }

        // ── GET /Cart/Checkout ── Trang thanh toán ────────────
        public async Task<IActionResult> Checkout()
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");
            if (!IsCustomer()) return RedirectToAction("Index", "Home");

            var cart = await GetOrCreateCartAsync(CurrentUserId);

            if (!cart.OrderItems.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index");
            }

            var user = await _db.Users.FindAsync(CurrentUserId);
            ViewBag.User = user;

            return View(cart);
        }

        // ── POST /Cart/Checkout ── Xác nhận đặt hàng ──────────
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string diaChiGiaoHang)
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");
            if (!IsCustomer()) return RedirectToAction("Index", "Home");

            if (string.IsNullOrWhiteSpace(diaChiGiaoHang))
            {
                TempData["Error"] = "Vui lòng nhập địa chỉ giao hàng.";
                return RedirectToAction("Checkout");
            }

            var cart = await GetOrCreateCartAsync(CurrentUserId);
            if (!cart.OrderItems.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index");
            }

            // Kiểm tra & trừ kho
            foreach (var item in cart.OrderItems)
            {
                var product = item.Product ?? await _db.Products.FindAsync(item.IdSanPham);
                if (product == null || product.SoLuongTonKho < item.SoLuongMua)
                {
                    TempData["Error"] = $"Sản phẩm \"{product?.TenSanPham}\" không đủ số lượng trong kho.";
                    return RedirectToAction("Index");
                }
            }

            foreach (var item in cart.OrderItems)
            {
                var product = item.Product ?? await _db.Products.FindAsync(item.IdSanPham);
                product!.SoLuongTonKho -= item.SoLuongMua;
                _db.Products.Update(product);
            }

            cart.DiaChiGiaoHang = diaChiGiaoHang.Trim();
            cart.TrangThaiThanhToan = "cho_thanh_toan";
            cart.NgayDatHang = DateTime.UtcNow;
            _db.StoreOrders.Update(cart);

            await _db.SaveChangesAsync();

            TempData["Success"] = "Đặt hàng thành công! Đơn hàng của bạn đang chờ thanh toán.";
            return RedirectToAction("OrderSuccess", new { id = cart.IdDonHang });
        }

        // ── GET /Cart/OrderSuccess/5 ── Xác nhận đặt hàng thành công ──
        public async Task<IActionResult> OrderSuccess(int id)
        {
            if (CurrentUserId == null) return RedirectToAction("Login", "Account");

            var order = await _db.StoreOrders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.IdDonHang == id && o.IdKhachHang == CurrentUserId);

            if (order == null) return NotFound();
            return View(order);
        }

        // ── GET /Cart/Count ── Số lượng sản phẩm trong giỏ (AJAX) ──
        [HttpGet]
        public async Task<IActionResult> Count()
        {
            if (CurrentUserId == null || !IsCustomer())
                return Json(new { count = 0 });

            var cart = await _db.StoreOrders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.IdKhachHang == CurrentUserId && o.TrangThaiThanhToan == TRANG_THAI_GIO_HANG);

            var count = cart?.OrderItems.Sum(oi => oi.SoLuongMua) ?? 0;
            return Json(new { count });
        }
    }
}
