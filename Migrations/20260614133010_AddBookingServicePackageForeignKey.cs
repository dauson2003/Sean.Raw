using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SeanRaw.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingServicePackageForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id_san_pham = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ten_san_pham = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    hang_may_tuong_thich = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    thong_so_chi_tiet = table.Column<string>(type: "text", nullable: true),
                    gia_ban_niem_yet = table.Column<decimal>(type: "numeric", nullable: false),
                    so_luong_ton_kho = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id_san_pham);
                });

            migrationBuilder.CreateTable(
                name: "question_bank",
                columns: table => new
                {
                    id_cau_hoi = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    noi_dung_cau_hoi = table.Column<string>(type: "text", nullable: false),
                    loai_cau_hoi = table.Column<string>(type: "text", nullable: false),
                    dap_an_dung_chuoi = table.Column<string>(type: "text", nullable: false),
                    do_kho_cau_hoi = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_bank", x => x.id_cau_hoi);
                });

            migrationBuilder.CreateTable(
                name: "service_packages",
                columns: table => new
                {
                    id_goi_dich_vu = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ten_goi = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    gia_tien = table.Column<decimal>(type: "numeric", nullable: false),
                    so_anh_duoc_sua = table.Column<int>(type: "integer", nullable: false),
                    mo_ta_quyen_loi = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_packages", x => x.id_goi_dich_vu);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id_nguoi_dung = table.Column<string>(type: "text", nullable: false),
                    ten_dang_nhap = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    dia_chi_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    mat_khau_bam = table.Column<string>(type: "text", nullable: false),
                    ho_ten_day_du = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    vai_tro_nguoi_dung = table.Column<string>(type: "text", nullable: false),
                    so_dien_thoai = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ngay_tao_tai_khoan = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    trang_thai_tai_khoan = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id_nguoi_dung);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id_log = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_nguoi_dung = table.Column<string>(type: "text", nullable: false),
                    hanh_vi = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    doi_tuong_bi_tac_dong = table.Column<string>(type: "text", nullable: true),
                    id_doi_tuong = table.Column<string>(type: "text", nullable: true),
                    dia_chi_ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    thiet_bi_dau_cuoi = table.Column<string>(type: "text", nullable: true),
                    thoi_gian_thao_tac = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.id_log);
                    table.ForeignKey(
                        name: "FK_audit_logs_users_id_nguoi_dung",
                        column: x => x.id_nguoi_dung,
                        principalTable: "users",
                        principalColumn: "id_nguoi_dung",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    id_booking = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_khach_hang = table.Column<string>(type: "text", nullable: false),
                    id_photographer = table.Column<string>(type: "text", nullable: false),
                    id_goi_dich_vu = table.Column<int>(type: "integer", nullable: false),
                    ngay_chup_thuc_te = table.Column<DateOnly>(type: "date", nullable: false),
                    khung_gio_chup = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    dia_diem_chup = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    trang_thai_booking = table.Column<string>(type: "text", nullable: false),
                    tong_tien_hop_dong = table.Column<decimal>(type: "numeric", nullable: false),
                    tien_da_dat_coc = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bookings", x => x.id_booking);
                    table.ForeignKey(
                        name: "FK_bookings_service_packages_id_goi_dich_vu",
                        column: x => x.id_goi_dich_vu,
                        principalTable: "service_packages",
                        principalColumn: "id_goi_dich_vu",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bookings_users_id_khach_hang",
                        column: x => x.id_khach_hang,
                        principalTable: "users",
                        principalColumn: "id_nguoi_dung",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bookings_users_id_photographer",
                        column: x => x.id_photographer,
                        principalTable: "users",
                        principalColumn: "id_nguoi_dung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "photographer_profiles",
                columns: table => new
                {
                    id_ho_so = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_nguoi_dung = table.Column<string>(type: "text", nullable: false),
                    so_can_cuoc_cong_dan = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    anh_chup_can_cuoc = table.Column<string>(type: "text", nullable: false),
                    trang_thai_xac_thuc = table.Column<string>(type: "text", nullable: false),
                    gioi_thieu_phong_cach = table.Column<string>(type: "text", nullable: true),
                    diem_uy_tin_trung_binh = table.Column<double>(type: "double precision", nullable: false),
                    so_du_vi_tien = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_photographer_profiles", x => x.id_ho_so);
                    table.ForeignKey(
                        name: "FK_photographer_profiles_users_id_nguoi_dung",
                        column: x => x.id_nguoi_dung,
                        principalTable: "users",
                        principalColumn: "id_nguoi_dung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_orders",
                columns: table => new
                {
                    id_don_hang = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_khach_hang = table.Column<string>(type: "text", nullable: false),
                    ngay_dat_hang = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tong_tien_hoa_don = table.Column<decimal>(type: "numeric", nullable: false),
                    trang_thai_thanh_toan = table.Column<string>(type: "text", nullable: false),
                    dia_chi_giao_hang = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_orders", x => x.id_don_hang);
                    table.ForeignKey(
                        name: "FK_store_orders_users_id_khach_hang",
                        column: x => x.id_khach_hang,
                        principalTable: "users",
                        principalColumn: "id_nguoi_dung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "albums",
                columns: table => new
                {
                    id_album = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_booking = table.Column<int>(type: "integer", nullable: false),
                    thu_muc_anh_tho_url = table.Column<string>(type: "text", nullable: false),
                    thu_muc_anh_sua_url = table.Column<string>(type: "text", nullable: true),
                    ma_truy_cap_bao_mat = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    trang_thai_mo_khoa = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_albums", x => x.id_album);
                    table.ForeignKey(
                        name: "FK_albums_bookings_id_booking",
                        column: x => x.id_booking,
                        principalTable: "bookings",
                        principalColumn: "id_booking",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    id_danh_gia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_booking = table.Column<int>(type: "integer", nullable: false),
                    so_sao_chat_luong = table.Column<short>(type: "smallint", nullable: false),
                    noi_dung_nhan_xet = table.Column<string>(type: "text", nullable: true),
                    ngay_tao_phan_hoi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.id_danh_gia);
                    table.ForeignKey(
                        name: "FK_reviews_bookings_id_booking",
                        column: x => x.id_booking,
                        principalTable: "bookings",
                        principalColumn: "id_booking",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_items",
                columns: table => new
                {
                    id_chi_tiet = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_don_hang = table.Column<int>(type: "integer", nullable: false),
                    id_san_pham = table.Column<int>(type: "integer", nullable: false),
                    so_luong_mua = table.Column<int>(type: "integer", nullable: false),
                    gia_giao_dich = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => x.id_chi_tiet);
                    table.ForeignKey(
                        name: "FK_order_items_products_id_san_pham",
                        column: x => x.id_san_pham,
                        principalTable: "products",
                        principalColumn: "id_san_pham",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_items_store_orders_id_don_hang",
                        column: x => x.id_don_hang,
                        principalTable: "store_orders",
                        principalColumn: "id_don_hang",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "selected_photos",
                columns: table => new
                {
                    id_anh_chon = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_album = table.Column<int>(type: "integer", nullable: false),
                    ten_file_anh = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ngay_chon = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AlbumIdAlbum = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_selected_photos", x => x.id_anh_chon);
                    table.ForeignKey(
                        name: "FK_selected_photos_albums_AlbumIdAlbum",
                        column: x => x.AlbumIdAlbum,
                        principalTable: "albums",
                        principalColumn: "id_album",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_albums_id_booking",
                table: "albums",
                column: "id_booking",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_id_nguoi_dung",
                table: "audit_logs",
                column: "id_nguoi_dung");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_id_goi_dich_vu",
                table: "bookings",
                column: "id_goi_dich_vu");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_id_khach_hang",
                table: "bookings",
                column: "id_khach_hang");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_id_photographer",
                table: "bookings",
                column: "id_photographer");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_id_don_hang",
                table: "order_items",
                column: "id_don_hang");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_id_san_pham",
                table: "order_items",
                column: "id_san_pham");

            migrationBuilder.CreateIndex(
                name: "IX_photographer_profiles_id_nguoi_dung",
                table: "photographer_profiles",
                column: "id_nguoi_dung",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reviews_id_booking",
                table: "reviews",
                column: "id_booking",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_selected_photos_AlbumIdAlbum",
                table: "selected_photos",
                column: "AlbumIdAlbum");

            migrationBuilder.CreateIndex(
                name: "IX_store_orders_id_khach_hang",
                table: "store_orders",
                column: "id_khach_hang");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "order_items");

            migrationBuilder.DropTable(
                name: "photographer_profiles");

            migrationBuilder.DropTable(
                name: "question_bank");

            migrationBuilder.DropTable(
                name: "reviews");

            migrationBuilder.DropTable(
                name: "selected_photos");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "store_orders");

            migrationBuilder.DropTable(
                name: "albums");

            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropTable(
                name: "service_packages");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
