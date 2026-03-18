using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace QLTT.Models
{
    /// <summary>
    /// Đại diện cho một chiến dịch quyên góp trong hệ thống.
    /// Chiến dịch chứa thông tin về mục đích quyên góp, mục tiêu tài chính, 
    /// người tạo, và danh sách những khoản đóng góp.
    /// </summary>
    public class Campaign
    {
        /// <summary>
        /// Mã chiến dịch duy nhất (khóa chính).
        /// Được tự động sinh bởi database.
        /// </summary>
        [Key]
        public int CampaignId { get; set; }

        /// <summary>
        /// Mã người dùng tạo chiến dịch (khóa ngoại).
        /// Liên kết đến bảng Users.
        /// </summary>
        [Required(ErrorMessage = "Chiến dịch phải có người tạo")]
        public int UserId { get; set; }

        /// <summary>
        /// Tham chiếu đến đối tượng User tạo chiến dịch.
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// Tiêu đề/tên chiến dịch.
        /// Bắt buộc, tối đa 200 ký tự.
        /// Ví dụ: "Giúp đỡ trẻ em khuyết tật"
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề chiến dịch")]
        [MaxLength(200, ErrorMessage = "Tiêu đề không vượt quá 200 ký tự")]
        public string Title { get; set; }

        /// <summary>
        /// Thông tin về người/gia đình cần nhận hỗ trợ.
        /// Gồm tên, địa chỉ, hoàn cảnh cụ thể.
        /// </summary>
        public string ReceiverInfo { get; set; }

        /// <summary>
        /// Nội dung chi tiết của chiến dịch.
        /// Mô tả lý do, mục đích, tình hình hiện tại của người cần giúp.
        /// Có thể chứa HTML formatting.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Đường dẫn hình ảnh đại diện của chiến dịch.
        /// Ví dụ: "/uploads/abc123def.jpg"
        /// Hình ảnh được lưu trong wwwroot/uploads/.
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Mục tiêu tài chính - số tiền cần quyên góp (tính bằng VND).
        /// Ví dụ: 5000000 (5 triệu đồng)
        /// </summary>
        [Range(1000, 999999999, ErrorMessage = "Mục tiêu tài chính phải lớn hơn 1.000 VND")]
        public decimal TargetAmount { get; set; }

        /// <summary>
        /// Số tiền gợi ý cho mỗi lần đóng góp (tính bằng VND).
        /// Dùng để hướng dẫn người quyên góp.
        /// Ví dụ: 100000 (100 ngàn đồng)
        /// </summary>
        public decimal SuggestedAmount { get; set; }

        /// <summary>
        /// Số tiền hiện đã nhận được từ các khoản đóng góp.
        /// Được cập nhật mỗi lần có khoản donation mới.
        /// Mặc định = 0 khi chiến dịch được tạo.
        /// </summary>
        public decimal CurrentAmount { get; set; } = 0;

        /// <summary>
        /// Trạng thái duyệt của chiến dịch.
        /// false = chỉ quản trị viên có thể duyệt chiến dịch
        /// true = chiến dịch đã được phê duyệt, công khai trên hệ thống
        /// </summary>
        public bool IsApproved { get; set; } = false;

        /// <summary>
        /// Trạng thái hoàn thành của chiến dịch.
        /// false = chiến dịch vẫn đang chương trình quyên góp
        /// true = chiến dịch đã hoàn thành, không còn nhận đóng góp
        /// </summary>
        public bool IsCompleted { get; set; } = false;

        /// <summary>
        /// Ngày bắt đầu chiến dịch.
        /// Nullable - có thể để trống.
        /// Mặc định được set là ngày tạo chiến dịch.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc chiến dịch.
        /// Nullable - có thể để trống.
        /// Sau ngày này, chiến dịch được tự động đóng.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Ngày và giờ chiến dịch được tạo.
        /// Được thiết lập tự động bởi hệ thống.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ==================== Mối quan hệ (Relationships) ====================

        /// <summary>
        /// Danh sách tất cả các khoản quyên góp cho chiến dịch này.
        /// Mối quan hệ One-to-Many: 1 Campaign → Many Donations.
        /// Khi chiến dịch bị xóa, tất cả donations liên quan cũng bị xóa (cascade delete).
        /// </summary>
        public ICollection<Donation> Donations { get; set; }

        /// <summary>
        /// Danh sách tất cả các bình luận trên chiến dịch này.
        /// Mối quan hệ One-to-Many: 1 Campaign → Many Comments.
        /// Khi chiến dịch bị xóa, tất cả comments liên quan cũng bị xóa (cascade delete).
        /// </summary>
        public ICollection<Comment> Comments { get; set; }

        // ==================== Hàm hỗ trợ ====================

        /// <summary>
        /// Tính phần trăm hoàn thành của chiến dịch.
        /// </summary>
        /// <returns>Phần trăm (0-100) dựa trên CurrentAmount và TargetAmount</returns>
        public decimal GetProgressPercentage()
        {
            if (TargetAmount <= 0) return 0;
            return Math.Min((CurrentAmount / TargetAmount) * 100, 100);
        }

        /// <summary>
        /// Kiểm tra xem chiến dịch có vượt quá mục tiêu tài chính không.
        /// </summary>
        /// <returns>true nếu CurrentAmount >= TargetAmount</returns>
        public bool IsTargetReached()
        {
            return CurrentAmount >= TargetAmount;
        }
    }
}
