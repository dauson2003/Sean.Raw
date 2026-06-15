using Microsoft.EntityFrameworkCore;
using SeanRaw.Web.Data;
using SeanRaw.Web.Models;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Đăng ký MVC ──────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ── 2. Kết nối PostgreSQL qua Entity Framework Core ─────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── 3. Session (lưu thông tin đăng nhập) ────────────────────
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ── Seed initial data on startup ────────────────────────────
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        // Tạo test users nếu chưa có
        if (!db.Users.Any())
        {
            var testUsers = new List<User>
            {
                new User
                {
                    IdNguoiDung = Guid.NewGuid().ToString(),
                    TenDangNhap = "admin",
                    DiaChiEmail = "admin@seanraw.com",
                    HoTenDayDu = "Admin System",
                    MatKhauBam = BCrypt.Net.BCrypt.HashPassword("123456"),
                    VaiTroNguoiDung = "admin",
                    SoDienThoai = "0123456789",
                    TrangThaiTaiKhoan = 1
                },
                new User
                {
                    IdNguoiDung = Guid.NewGuid().ToString(),
                    TenDangNhap = "photographer1",
                    DiaChiEmail = "photographer@seanraw.com",
                    HoTenDayDu = "Nguyễn Văn A - Nhiếp Ảnh Gia",
                    MatKhauBam = BCrypt.Net.BCrypt.HashPassword("123456"),
                    VaiTroNguoiDung = "photographer",
                    SoDienThoai = "0912345678",
                    TrangThaiTaiKhoan = 1
                },
                new User
                {
                    IdNguoiDung = Guid.NewGuid().ToString(),
                    TenDangNhap = "customer1",
                    DiaChiEmail = "customer@seanraw.com",
                    HoTenDayDu = "Trần Thị B - Khách Hàng",
                    MatKhauBam = BCrypt.Net.BCrypt.HashPassword("123456"),
                    VaiTroNguoiDung = "client",
                    SoDienThoai = "0987654321",
                    TrangThaiTaiKhoan = 1
                }
            };

            db.Users.AddRange(testUsers);
            await db.SaveChangesAsync();
            Console.WriteLine("✅ Seeding: Created 3 test users");
        }
        else
        {
            Console.WriteLine("ℹ️  Seeding: Users already exist, skipping...");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Seeding error: {ex.Message}");
    Console.WriteLine($"   {ex.InnerException?.Message}");
}

// ── Middleware pipeline ──────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();       // ← phải đặt trước UseAuthorization
app.UseAuthorization();

// ── Route mặc định ──────────────────────────────────────────
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
