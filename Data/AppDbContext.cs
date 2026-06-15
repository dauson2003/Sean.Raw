using Microsoft.EntityFrameworkCore;
using SeanRaw.Web.Models;

namespace SeanRaw.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<PhotographerProfile> PhotographerProfiles { get; set; }
        public DbSet<ServicePackage> ServicePackages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<SelectedPhoto> SelectedPhotos { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StoreOrder> StoreOrders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<QuestionBank> QuestionBank { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Tên bảng ────────────────────────────────────────
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<PhotographerProfile>().ToTable("photographer_profiles");
            modelBuilder.Entity<ServicePackage>().ToTable("service_packages");
            modelBuilder.Entity<Booking>().ToTable("bookings");
            modelBuilder.Entity<Album>().ToTable("albums");
            modelBuilder.Entity<SelectedPhoto>().ToTable("selected_photos");
            modelBuilder.Entity<Review>().ToTable("reviews");
            modelBuilder.Entity<Product>().ToTable("products");
            modelBuilder.Entity<StoreOrder>().ToTable("store_orders");
            modelBuilder.Entity<OrderItem>().ToTable("order_items");
            modelBuilder.Entity<QuestionBank>().ToTable("question_bank");
            modelBuilder.Entity<AuditLog>().ToTable("audit_logs");

            // ── Booking → KhachHang (FK: id_khach_hang) ─────────
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.KhachHang)
                .WithMany(u => u.BookingsAsClient)
                .HasForeignKey(b => b.IdKhachHang)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Booking → Photographer (FK: id_photographer) ─────
            // User KHÔNG có nav-prop ngược lại → WithMany() không tham số
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Photographer)
                .WithMany()
                .HasForeignKey(b => b.IdPhotographer)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Booking → ServicePackage (FK: id_goi_dich_vu) ────
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.ServicePackage)
                .WithMany(sp => sp.Bookings)
                .HasForeignKey(b => b.IdGoiDichVu)
                .OnDelete(DeleteBehavior.Restrict);

            // ── 1:1 PhotographerProfile ↔ User ───────────────────
            modelBuilder.Entity<PhotographerProfile>()
                .HasOne(p => p.User)
                .WithOne(u => u.PhotographerProfile)
                .HasForeignKey<PhotographerProfile>(p => p.IdNguoiDung);

            // ── 1:1 Album ↔ Booking ───────────────────────────────
            modelBuilder.Entity<Album>()
                .HasOne(a => a.Booking)
                .WithOne(b => b.Album)
                .HasForeignKey<Album>(a => a.IdBooking);

            // ── 1:1 Review ↔ Booking ──────────────────────────────
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Booking)
                .WithOne(b => b.Review)
                .HasForeignKey<Review>(r => r.IdBooking);

            // ── OrderItem → StoreOrder ────────────────────────────
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.StoreOrder)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.IdDonHang);

            // ── OrderItem → Product ───────────────────────────────
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.IdSanPham);

            // ── StoreOrder → User ─────────────────────────────────
            modelBuilder.Entity<StoreOrder>()
                .HasOne(o => o.KhachHang)
                .WithMany(u => u.StoreOrders)
                .HasForeignKey(o => o.IdKhachHang)
                .OnDelete(DeleteBehavior.Restrict);

            // ── AuditLog → User ───────────────────────────────────
            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.IdNguoiDung)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}