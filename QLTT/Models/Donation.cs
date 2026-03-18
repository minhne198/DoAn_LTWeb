using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTT.Models
{
    /// <summary>
    /// Đại diện cho một khoản đóng góp/quyên góp từ người dùng cho chiến dịch.
    /// Mỗi khoản donation ghi lại thông tin người quyên góp, chiến dịch, số tiền, 
    /// và lời nhắn từ nhà quyên góp.
    /// </summary>
    public class Donation
    {
        /// <summary>
        /// Mã khoản đóng góp duy nhất (khóa chính).
        /// Được tự động sinh bởi database.
        /// </summary>
        [Key]
        public int DonationId { get; set; }

        /// <summary>
        /// Mã chiến dịch mà khoản đóng góp này dành cho (khóa ngoại).
        /// Liên kết đến bảng Campaigns.
        /// </summary>
        [Required(ErrorMessage = "Khoản đóng góp phải liên kết đến một chiến dịch")]
        public int CampaignId { get; set; }

        /// <summary>
        /// Tham chiếu đến đối tượng Campaign.
        /// Khi chiến dịch bị xóa, khoản donation này cũng bị xóa (cascade delete).
        /// </summary>
        [ForeignKey("CampaignId")]
        public Campaign Campaign { get; set; }

        /// <summary>
        /// Mã người dùng thực hiện khoản đóng góp (khóa ngoại).
        /// Liên kết đến bảng Users.
        /// </summary>
        [Required(ErrorMessage = "Khoản đóng góp phải có người quyên góp")]
        public int UserId { get; set; }

        /// <summary>
        /// Tham chiếu đến đối tượng User.
        /// Người quyên góp.
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// Số tiền quyên góp (tính bằng VND).
        /// Bắt buộc nhập, phải > 0.
        /// Ví dụ: 100000 (100 ngàn đồng)
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập số tiền quyên góp")]
        [Range(1000, 999999999, ErrorMessage = "Số tiền phải lớn hơn 1.000 VND")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Lời nhắn hoặc lời chúc từ người quyên góp.
        /// Không bắt buộc, tối đa 500 ký tự.
        /// Ví dụ: "Hy vọng em sớm khỏe mạnh"
        /// </summary>
        [MaxLength(500, ErrorMessage = "Lời nhắn không vượt quá 500 ký tự")]
        public string Message { get; set; }

        /// <summary>
        /// Trạng thái của khoản đóng góp.
        /// Giá trị mặc định: "Pending" (chờ xử lý/xác nhận)
        /// Các trạng thái có thể: "Pending", "Confirmed", "Rejected", "Completed"
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Ngày và giờ khoản đóng góp được thực hiện.
        /// Được thiết lập tự động bởi hệ thống.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Ngày tạo

        public DateTime DonatedAt { get; set; } = DateTime.Now; // Ngày đóng góp
    }
}
