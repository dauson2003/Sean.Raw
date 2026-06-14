using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeanRaw.Web.Models
{
    // ═══════════════════════════════════════════════════════════
    // MODEL 1: User — NGUOI_DUNG
    // ═══════════════════════════════════════════════════════════
    public class User
    {
        [Key]
        [Column("id_nguoi_dung")]
        public string IdNguoiDung { get; set; } = Guid.NewGuid().ToString();

        [Column("ten_dang_nhap")]
        [Required, MaxLength(256)]
        public string TenDangNhap { get; set; } = "";

        [Column("dia_chi_email")]
        [Required, EmailAddress, MaxLength(256)]
        public string DiaChiEmail { get; set; } = "";

        [Column("mat_khau_bam")]
        [Required]
        public string MatKhauBam { get; set; } = "";

        [Column("ho_ten_day_du")]
        [Required, MaxLength(200)]
        public string HoTenDayDu { get; set; } = "";

        [Column("vai_tro_nguoi_dung")]
        [Required]
        public string VaiTroNguoiDung { get; set; } = "client"; // admin|staff|photographer|client

        [Column("so_dien_thoai")]
        [Required, MaxLength(20)]
        public string SoDienThoai { get; set; } = "";

        [Column("ngay_tao_tai_khoan")]
        public DateTime NgayTaoTaiKhoan { get; set; } = DateTime.UtcNow;

        [Column("trang_thai_tai_khoan")]
        public short TrangThaiTaiKhoan { get; set; } = 1; // 1=hoạt động, 0=bị khóa

        // Navigation properties
        public PhotographerProfile? PhotographerProfile { get; set; }
        public ICollection<Booking> BookingsAsClient { get; set; } = new List<Booking>();
        public ICollection<StoreOrder> StoreOrders { get; set; } = new List<StoreOrder>();
    }

    // ═══════════════════════════════════════════════════════════
    // MODEL 2: PhotographerProfile — HO_SO_THO_ANH
    // ═══════════════════════════════════════════════════════════
    public class PhotographerProfile
    {
        [Key]
        [Column("id_ho_so")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdHoSo { get; set; }

        [Column("id_nguoi_dung")]
        [Required]
        public string IdNguoiDung { get; set; } = "";

        [Column("so_can_cuoc_cong_dan")]
        [Required, MaxLength(20)]
        public string SoCanCuocCongDan { get; set; } = "";

        [Column("anh_chup_can_cuoc")]
        [Required]
        public string AnhChupCanCuoc { get; set; } = "";

        [Column("trang_thai_xac_thuc")]
        public string TrangThaiXacThuc { get; set; } = "cho_duyet"; // cho_duyet|da_xac_thuc|tu_choi

        [Column("gioi_thieu_phong_cach")]
        public string? GioiThieuPhongCach { get; set; }

        [Column("diem_uy_tin_trung_binh")]
        public double DiemUyTinTrungBinh { get; set; } = 5.0;

        [Column("so_du_vi_tien")]
        public decimal SoDuViTien { get; set; } = 0;

        // Navigation
        public User User { get; set; } = null!;
    }

    // ═══════════════════════════════════════════════════════════
    // MODEL 3: ServicePackage — GOI_DICH_VU
    // ═══════════════════════════════════════════════════════════
    public class ServicePackage
    {
        [Key]
        [Column("id_goi_dich_vu")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdGoiDichVu { get; set; }

        [Column("ten_goi")]
        [Required, MaxLength(500)]
        public string TenGoi { get; set; } = "";

        [Column("gia_tien")]
        [Required]
        public decimal GiaTien { get; set; }

        [Column("so_anh_duoc_sua")]
        public int SoAnhDuocSua { get; set; }

        [Column("mo_ta_quyen_loi")]
        public string? MoTaQuyenLoi { get; set; }

        // Navigation
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

    // ═══════════════════════════════════════════════════════════
    // MODEL 4: Booking — LICH_DAT_CHUP
    // ═══════════════════════════════════════════════════════════
    public class Booking
    {
        [Key]
        [Column("id_booking")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdBooking { get; set; }

        [Column("id_khach_hang")]
        [Required]
        public string IdKhachHang { get; set; } = "";

        [Column("id_photographer")]
        [Required]
        public string IdPhotographer { get; set; } = "";

        [Column("id_goi_dich_vu")]
        public int IdGoiDichVu { get; set; }

        [Column("ngay_chup_thuc_te")]
        [Required]
        public DateOnly NgayChupThucTe { get; set; }

        [Column("khung_gio_chup")]
        [Required, MaxLength(50)]
        public string KhungGioChup { get; set; } = "";

        [Column("dia_diem_chup")]
        [Required, MaxLength(500)]
        public string DiaDiemChup { get; set; } = "";

        [Column("trang_thai_booking")]
        public string TrangThaiBooking { get; set; } = "cho_dat_coc";

        [Column("tong_tien_hop_dong")]
        public decimal TongTienHopDong { get; set; }

        [Column("tien_da_dat_coc")]
        public decimal TienDaDatCoc { get; set; } = 0;

        // Navigation
        public User KhachHang { get; set; } = null!;
        public User Photographer { get; set; } = null!;
        public ServicePackage ServicePackage { get; set; } = null!;
        public Album? Album { get; set; }
        public Review? Review { get; set; }
    }

    // ═══════════════════════════════════════════════════════════
    // MODEL 5: Album — KHO_ANH_SO
    // ═══════════════════════════════════════════════════════════
    public class Album
    {
        [Key]
        [Column("id_album")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAlbum { get; set; }

        [Column("id_booking")]
        public int IdBooking { get; set; }

        [Column("thu_muc_anh_tho_url")]
        [Required]
        public string ThuMucAnhThoUrl { get; set; } = "";

        [Column("thu_muc_anh_sua_url")]
        public string? ThuMucAnhSuaUrl { get; set; }

        [Column("ma_truy_cap_bao_mat")]
        [Required, MaxLength(64)]
        public string MaTruyCap { get; set; } = Guid.NewGuid().ToString("N")[..32];

        [Column("trang_thai_mo_khoa")]
        public short TrangThaiMoKhoa { get; set; } = 0; // 0=khóa, 1=đã mở

        // Navigation
        public Booking Booking { get; set; } = null!;
        public ICollection<SelectedPhoto> SelectedPhotos { get; set; } = new List<SelectedPhoto>();
    }

    // ═══════════════════════════════════════════════════════════
    // MODEL 6: SelectedPhoto — ANH_KHACH_CHON
    // ═══════════════════════════════════════════════════════════
    public class SelectedPhoto
    {
        [Key]
        [Column("id_anh_chon")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAnhChon { get; set; }

        [Column("id_album")]
        public int IdAlbum { get; set; }

        [Column("ten_file_anh")]
        [Required, MaxLength(256)]
        public string TenFileAnh { get; set; } = "";

        [Column("ngay_chon")]
        public DateTime NgayChon { get; set; } = DateTime.UtcNow;

        // Navigation
        public Album Album { get; set; } = null!;
    }

    // ═══════════════════════════════════════════════════════════
    // MODEL 7: Review — DANH_GIA_PHAN_HOI
    // ═══════════════════════════════════════════════════════════
    public class Review
    {
        [Key]
        [Column("id_danh_gia")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdDanhGia { get; set; }

        [Column("id_booking")]
        public int IdBooking { get; set; }

        [Column("so_sao_chat_luong")]
        [Range(1, 5)]
        public short SoSaoChatLuong { get; set; }

        [Column("noi_dung_nhan_xet")]
        public string? NoiDungNhanXet { get; set; }

        [Column("ngay_tao_phan_hoi")]
        public DateTime NgayTaoPhanHoi { get; set; } = DateTime.UtcNow;

        // Navigation
        public Booking Booking { get; set; } = null!;
    }

    // ═══════════════════════════════════════════════════════════
    // MODEL 8: Product — PHU_KIEN_MAY_ANH
    // ═══════════════════════════════════════════════════════════
    public class Product
    {
        [Key]
        [Column("id_san_pham")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdSanPham { get; set; }

        [Column("ten_san_pham")]
        [Required, MaxLength(500)]
        public string TenSanPham { get; set; } = "";

        [Column("hang_may_tuong_thich")]
        [Required, MaxLength(100)]
        public string HangMayTuongThich { get; set; } = "";

        [Column("thong_so_chi_tiet")]
        public string? ThongSoChiTiet { get; set; }

        [Column("gia_ban_niem_yet")]
        public decimal GiaBanNiemYet { get; set; }

        [Column("so_luong_ton_kho")]
        public int SoLuongTonKho { get; set; } = 0;

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        // Navigation
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    // ═══════════════════════════════════════════════════════════
    // MODEL 9: StoreOrder — DON_HANG_PHU_KIEN
    // ═══════════════════════════════════════════════════════════
    public class StoreOrder
    {
        [Key]
        [Column("id_don_hang")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdDonHang { get; set; }

        [Column("id_khach_hang")]
        [Required]
        public string IdKhachHang { get; set; } = "";

        [Column("ngay_dat_hang")]
        public DateTime NgayDatHang { get; set; } = DateTime.UtcNow;

        [Column("tong_tien_hoa_don")]
        public decimal TongTienHoaDon { get; set; }

        [Column("trang_thai_thanh_toan")]
        public string TrangThaiThanhToan { get; set; } = "cho_thanh_toan";

        [Column("dia_chi_giao_hang")]
        [Required, MaxLength(500)]
        public string DiaChiGiaoHang { get; set; } = "";

        // Navigation
        public User KhachHang { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    // ═══════════════════════════════════════════════════════════
    // MODEL 10: OrderItem — CHI_TIET_DON_HANG
    // ═══════════════════════════════════════════════════════════
    public class OrderItem
    {
        [Key]
        [Column("id_chi_tiet")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdChiTiet { get; set; }

        [Column("id_don_hang")]
        public int IdDonHang { get; set; }

        [Column("id_san_pham")]
        public int IdSanPham { get; set; }

        [Column("so_luong_mua")]
        public int SoLuongMua { get; set; } = 1;

        [Column("gia_giao_dich")]
        public decimal GiaGiaoDich { get; set; } // snapshot giá lúc mua

        // Navigation
        public StoreOrder StoreOrder { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }

    // ═══════════════════════════════════════════════════════════
    // MODEL 11: QuestionBank — NGAN_HANG_CAU_HOI
    // ═══════════════════════════════════════════════════════════
    public class QuestionBank
    {
        [Key]
        [Column("id_cau_hoi")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCauHoi { get; set; }

        [Column("noi_dung_cau_hoi")]
        [Required]
        public string NoiDungCauHoi { get; set; } = "";

        [Column("loai_cau_hoi")]
        [Required]
        public string LoaiCauHoi { get; set; } = "mot_dap_an";

        [Column("dap_an_dung_chuoi")]
        [Required]
        public string DapAnDungChuoi { get; set; } = "";

        [Column("do_kho_cau_hoi")]
        public string DoKhoCauHoi { get; set; } = "trung_binh";
    }

    // ═══════════════════════════════════════════════════════════
    // MODEL 12: AuditLog — NHAT_KY_HE_THONG
    // ═══════════════════════════════════════════════════════════
    public class AuditLog
    {
        [Key]
        [Column("id_log")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdLog { get; set; }

        [Column("id_nguoi_dung")]
        [Required]
        public string IdNguoiDung { get; set; } = "";

        [Column("hanh_vi")]
        [Required, MaxLength(200)]
        public string HanhVi { get; set; } = "";

        [Column("doi_tuong_bi_tac_dong")]
        public string? DoiTuongBiTacDong { get; set; }

        [Column("id_doi_tuong")]
        public string? IdDoiTuong { get; set; }

        [Column("dia_chi_ip")]
        [Required, MaxLength(45)]
        public string DiaChiIp { get; set; } = "";

        [Column("thiet_bi_dau_cuoi")]
        public string? ThietBiDauCuoi { get; set; }

        [Column("thoi_gian_thao_tac")]
        public DateTime ThoiGianThaoTac { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; } = null!;
    }

    // ═══════════════════════════════════════════════════════════
    // ViewModels — Dùng cho form đăng nhập/đặt lịch
    // ═══════════════════════════════════════════════════════════
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = "";
    }

    public class BookingViewModel
    {
        [Required]
        public string IdPhotographer { get; set; } = "";

        [Required]
        public int IdGoiDichVu { get; set; }

        [Required]
        public DateOnly NgayChup { get; set; }

        [Required]
        public string KhungGio { get; set; } = "";

        [Required]
        public string DiaDiem { get; set; } = "";
    }
}
