-- ═══════════════════════════════════════════════════════════
-- FIX PASSWORDS: Hash tất cả existing user passwords với BCrypt
-- ═══════════════════════════════════════════════════════════
-- CÁCH DÙNG:
-- 1. Mở pgAdmin hoặc psql
-- 2. Kết nối vào database seanraw_db
-- 3. Chạy script này

-- Lưu ý: Đây là solution tạm thời cho test users
-- Mật khẩu mặc định sẽ được set thành: 123456
-- Bạn cần đổi mật khẩu sau khi đăng nhập

-- ─────────────────────────────────────────────────────────
-- PHƯƠNG ÁN 1: Xóa tất cả users cũ + tạo 3 test users mới
-- ─────────────────────────────────────────────────────────
DELETE FROM users;

INSERT INTO users (
    id_nguoi_dung, 
    ten_dang_nhap, 
    dia_chi_email, 
    mat_khau_bam,
    ho_ten_day_du,
    vai_tro_nguoi_dung,
    so_dien_thoai,
    ngay_tao_tai_khoan,
    trang_thai_tai_khoan
) VALUES 
(
    gen_random_uuid(),
    'admin',
    'admin@seanraw.com',
    '$2a$11$RhcirV.vdAe3K3xLvMzwn.hqFxU.8F5Y8hkG7/X0N4aT8V7lWAEKS', -- 123456 hashed
    'Admin System',
    'admin',
    '0123456789',
    NOW(),
    1
),
(
    gen_random_uuid(),
    'photographer1',
    'photographer@seanraw.com',
    '$2a$11$RhcirV.vdAe3K3xLvMzwn.hqFxU.8F5Y8hkG7/X0N4aT8V7lWAEKS', -- 123456 hashed
    'Nguyễn Văn A - Nhiếp Ảnh Gia',
    'photographer',
    '0912345678',
    NOW(),
    1
),
(
    gen_random_uuid(),
    'customer1',
    'customer@seanraw.com',
    '$2a$11$RhcirV.vdAe3K3xLvMzwn.hqFxU.8F5Y8hkG7/X0N4aT8V7lWAEKS', -- 123456 hashed
    'Trần Thị B - Khách Hàng',
    'client',
    '0987654321',
    NOW(),
    1
);

-- ─────────────────────────────────────────────────────────
-- PHƯƠNG ÁN 2: Hash lại mật khẩu toàn bộ users hiện tại
-- (Chỉ dùng nếu muốn giữ lại users cũ)
-- ─────────────────────────────────────────────────────────
-- UPDATE users
-- SET mat_khau_bam = '$2a$11$RhcirV.vdAe3K3xLvMzwn.hqFxU.8F5Y8hkG7/X0N4aT8V7lWAEKS'
-- WHERE mat_khau_bam IS NOT NULL AND mat_khau_bam NOT LIKE '$2%';

-- ─────────────────────────────────────────────────────────
-- VERIFY: Kiểm tra kết quả
-- ─────────────────────────────────────────────────────────
SELECT 
    id_nguoi_dung,
    ten_dang_nhap,
    dia_chi_email,
    ho_ten_day_du,
    vai_tro_nguoi_dung,
    trang_thai_tai_khoan
FROM users
ORDER BY ngay_tao_tai_khoan DESC;
