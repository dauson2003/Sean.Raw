using Microsoft.EntityFrameworkCore;
using SeanRaw.Web.Data;

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
