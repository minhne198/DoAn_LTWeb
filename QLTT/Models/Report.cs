using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTT.Models
{
    /// <summary>
    /// Đại diện cho một báo cáo/tố cáo về chiến dịch không hợp lệ hoặc vi phạm quy tắc.
    /// Quản trị viên sẽ xem xét và xử lý các báo cáo.
    /// </summary>
    public class Report
    {
        /// <summary>
        /// Mã báo cáo duy nhất (khóa chính).
        /// Được tự động sinh bởi database.
        /// </summary>
        [Key]
        public int ReportId { get; set; }

        /// <summary>
        /// Mã chiến dịch bị báo cáo (khóa ngoại).
        /// Liên kết đến bảng Campaigns.
        /// </summary>
        [Required(ErrorMessage = "Báo cáo phải liên kết đến một chiến dịch")]
        public int CampaignId { get; set; }

        /// <summary>
        /// Tham chiếu đến đối tượng Campaign.
        /// Chiến dịch bị báo cáo.
        /// Khi chiến dịch bị xóa, báo cáo của nó cũng bị xóa (cascade delete).
        /// </summary>
        [ForeignKey("CampaignId")]
        public Campaign Campaign { get; set; }

        /// <summary>
        /// Mã người dùng báo cáo (khóa ngoại).
        /// Liên kết đến bảng Users.
        /// </summary>
        [Required(ErrorMessage = "Báo cáo phải có người thực hiện")]
        public int UserId { get; set; }

        /// <summary>
        /// Tham chiếu đến đối tượng User.
        /// Người tố cáo chiến dịch.
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// Lý do báo cáo/tố cáo.
        /// Bắt buộc nhập, tối đa 255 ký tự.
        /// Ví dụ: "Chiến dịch giả mạo, thông tin không chính xác"
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập lý do báo cáo")]
        [MaxLength(255, ErrorMessage = "Lý do không vượt quá 255 ký tự")]
        public string Reason { get; set; }

        /// <summary>
        /// Trạng thái xử lý của báo cáo.
        /// false = chưa được xử lý/xem xét
        /// true = đã xử lý (có thể là duyệt hoặc từ chối)
        /// </summary>
        public bool IsHandled { get; set; } = false;

        /// <summary>
        /// Ngày và giờ báo cáo được gửi.
        /// Được thiết lập tự động bởi hệ thống.
        /// </summary>
        public DateTime ReportedAt { get; set; } = DateTime.Now;
    }
}
