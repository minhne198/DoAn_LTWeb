using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QLTT.Models
{
    /// <summary>
    /// Đại diện cho một người dùng trong hệ thống.
    /// Mỗi người dùng có thể tạo chiến dịch quyên góp, đóng góp cho chiến dịch khác, 
    /// hoặc quản lý hệ thống nếu là admin.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Mã người dùng duy nhất (khóa chính).
        /// </summary>
        [Key]
        public int UserId { get; set; }

        /// <summary>
        /// Họ và tên đầy đủ của người dùng.
        /// Bắt buộc điền, tối đa 100 ký tự.
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [MaxLength(100, ErrorMessage = "Họ tên không vượt quá 100 ký tự")]
        public string FullName { get; set; }

        /// <summary>
        /// Địa chỉ email của người dùng (dùng để đăng nhập).
        /// Bắt buộc là email hợp lệ, dùng làm tên đăng nhập.
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [MaxLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// Hash mật khẩu của user (được mã hóa bằng thuật toán an toàn như bcrypt).
        /// KHÔNG bao giờ lưu mật khẩu dạng văn bản thường.
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string PasswordHash { get; set; }

        /// <summary>
        /// Vai trò của người dùng: "admin" hoặc "user".
        /// Quyết định quyền truy cập vào các chức năng.
        /// </summary>
        [Required]
        public string Role { get; set; } = "user";

        /// <summary>
        /// Cờ cho biết tài khoản có bị khóa hay không.
        /// Tài khoản bị khóa sẽ không thể đăng nhập.
        /// </summary>
        public bool IsLocked { get; set; } = false;

        /// <summary>
        /// Ngày và giờ tạo tài khoản.
        /// Được thiết lập tự động khi user được tạo.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ==================== Mối quan hệ (Relationships) ====================

        /// <summary>
        /// Danh sách các chiến dịch quyên góp mà người dùng này tạo ra.
        /// Mối quan hệ One-to-Many: 1 User → Many Campaigns.
        /// </summary>
        public ICollection<Campaign> Campaigns { get; set; }

        /// <summary>
        /// Danh sách các khoản đóng góp mà người dùng này thực hiện.
        /// Mối quan hệ One-to-Many: 1 User → Many Donations.
        /// </summary>
        public ICollection<Donation> Donations { get; set; }
    }
}
