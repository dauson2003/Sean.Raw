using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeanRaw.Web.Data;

namespace SeanRaw.Web.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public CartCountViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            int count = 0;

            if (userId != null && role == "client")
            {
                count = await _db.OrderItems
                    .Where(oi => oi.StoreOrder.IdKhachHang == userId
                                && oi.StoreOrder.TrangThaiThanhToan == "trong_gio")
                    .SumAsync(oi => oi.SoLuongMua);
            }

            return View(count);
        }
    }
}
