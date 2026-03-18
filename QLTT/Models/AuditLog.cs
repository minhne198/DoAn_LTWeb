using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTT.Models
{
    /// <summary>
    /// Ghi lại lịch sử các hành động của người dùng trong hệ thống.
    /// Dùng cho mục đích kiểm tra an ninh, truy dò, và tuân thủ pháp lý.
    /// </summary>
    public class AuditLog
    {
        /// <summary>
        /// Mã log duy nhất (khóa chính).
        /// Được tự động sinh bởi database.
        /// </summary>
        [Key]
        public int LogId { get; set; }

        /// <summary>
        /// Mã người dùng thực hiện hành động (khóa ngoại).
        /// Liên kết đến bảng Users.
        /// </summary>
        [Required(ErrorMessage = "Audit log phải ghi nhận hành động của một người dùng")]
        public int UserId { get; set; }

        /// <summary>
        /// Tham chiếu đến đối tượng User.
        /// Người thực hiện hành động được ghi lại.
        /// Khi user bị xóa, log của họ cũng bị xóa (cascade delete, tùy theo chính sách).
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// Mô tả chi tiết hành động được thực hiện.
        /// Ví dụ: "Tạo chiến dịch 'Giúp em lan' ", 
        ///        "Đăng nhập vào hệ thống",
        ///        "Cập nhật hồ sơ cá nhân",
        ///        "Xóa bình luận từ user 123"
        /// </summary>
        [MaxLength(500, ErrorMessage = "Mô tả hành động không vượt quá 500 ký tự")]
        public string Action { get; set; }

        /// <summary>
        /// Ngày, giờ, phút, giây hành động được thực hiện.
        /// Được thiết lập tự động bởi hệ thống.
        /// Dùng để truy dò thời gian xảy ra sự việc.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
