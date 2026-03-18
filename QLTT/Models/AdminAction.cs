using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTT.Models
{
    /// <summary>
    /// Ghi lại các hành động quản trị của admin trong hệ thống.
    /// Ví dụ: duyệt chiến dịch, khóa/mở khóa tài khoản, xóa báo cáo, v.v.
    /// Dùng để kiểm soát và truy dò công việc của admin.
    /// </summary>
    public class AdminAction
    {
        /// <summary>
        /// Mã hành động admin duy nhất (khóa chính).
        /// Được tự động sinh bởi database.
        /// </summary>
        [Key]
        public int AdminActionId { get; set; }

        /// <summary>
        /// Mã admin thực hiện hành động (khóa ngoại).
        /// Liên kết đến bảng Users, chỉ admin mới thực hiện được.
        /// </summary>
        [Required(ErrorMessage = "Hành động admin phải được thực hiện bởi một admin")]
        public int AdminId { get; set; }

        /// <summary>
        /// Tham chiếu đến đối tượng User (admin).
        /// Admin thực hiện hành động.
        /// Khi admin bị xóa, lịch sử hành động của họ có thể được giữ lại (restrict delete).
        /// </summary>
        [ForeignKey("AdminId")]
        public User Admin { get; set; }

        /// <summary>
        /// Loại hành động được thực hiện.
        /// Các giá trị có thể: 
        /// - "Approved" (duyệt chiến dịch),
        /// - "Rejected" (từ chối chiến dịch),
        /// - "LockUser" (khóa tài khoản),
        /// - "UnlockUser" (mở khóa tài khoản),
        /// - "DeleteCampaign" (xóa chiến dịch),
        /// - "DeleteComment" (xóa bình luận),
        /// - "ResolveReport" (giải quyết báo cáo)
        /// </summary>
        [MaxLength(50, ErrorMessage = "Loại hành động không vượt quá 50 ký tự")]
        public string ActionType { get; set; }

        /// <summary>
        /// ID của mục tiêu bị tác động bởi hành động (nếu có).
        /// Nullable - có thể để trống nếu hành động không liên kết đến một đối tượng cụ thể.
        /// Ví dụ: Nếu xóa Campaign ID 5 thì TargetId = 5
        /// Hoặc nếu khóa User ID 10 thì TargetId = 10
        /// </summary>
        public int? TargetId { get; set; }

        /// <summary>
        /// Ghi chú hoặc lý do thực hiện hành động.
        /// Giúp admin khác hiểu tại sao hành động được thực hiện.
        /// Ví dụ: "Chiến dịch giả mạo, thông tin không chính xác",
        ///        "Tài khoản vi phạm quy tắc cộng đồng"
        /// </summary>
        [MaxLength(255, ErrorMessage = "Ghi chú không vượt quá 255 ký tự")]
        public string Notes { get; set; }

        /// <summary>
        /// Ngày, giờ, phút, giây thực hiện hành động admin.
        /// Được thiết lập tự động bởi hệ thống.
        /// </summary>
        public DateTime ActionAt { get; set; } = DateTime.Now;
    }
}
