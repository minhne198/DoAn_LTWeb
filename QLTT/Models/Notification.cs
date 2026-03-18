using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTT.Models
{
    /// <summary>
    /// Đại diện cho một thông báo gửi đến người dùng.
    /// Thông báo có thể là: cập nhật chiến dịch, thông báo khoản donation, duyệt chiến dịch, v.v.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Mã thông báo duy nhất (khóa chính).
        /// Được tự động sinh bởi database.
        /// </summary>
        [Key]
        public int NotificationId { get; set; }

        /// <summary>
        /// Mã người dùng nhận thông báo (khóa ngoại).
        /// Liên kết đến bảng Users.
        /// </summary>
        [Required(ErrorMessage = "Thông báo phải gửi đến một người dùng")]
        public int UserId { get; set; }

        /// <summary>
        /// Tham chiếu đến đối tượng User.
        /// Người nhận thông báo.
        /// Khi user bị xóa, thông báo của họ cũng bị xóa (cascade delete).
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// Nội dung của thông báo.
        /// Bắt buộc nhập, tối đa 500 ký tự.
        /// Ví dụ: "Chiến dịch 'Giúp em Lan' của bạn được duyệt!" 
        /// hoặc "Bạn nhận được 100.000 VND từ người quyên góp ẩn danh"
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập nội dung thông báo")]
        [MaxLength(500, ErrorMessage = "Nội dung không vượt quá 500 ký tự")]
        public string Content { get; set; }

        /// <summary>
        /// Trạng thái đã đọc của thông báo.
        /// false = thông báo chưa được đọc (mới)
        /// true = thông báo đã được đọc
        /// Dùng để hiển thị badge "thông báo mới" trên giao diện.
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Ngày và giờ thông báo được tạo/gửi.
        /// Được thiết lập tự động bởi hệ thống.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
