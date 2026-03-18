using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTT.Models
{
    /// <summary>
    /// Đại diện cho mối quan hệ "theo dõi" giữa người dùng và chiến dịch.
    /// Khi user theo dõi chiến dịch, họ sẽ nhận được thông báo về cập nhật mới.
    /// </summary>
    public class Follow
    {
        /// <summary>
        /// Mã theo dõi duy nhất (khóa chính).
        /// Được tự động sinh bởi database.
        /// </summary>
        [Key]
        public int FollowId { get; set; }

        /// <summary>
        /// Mã người dùng theo dõi (khóa ngoại).
        /// Liên kết đến bảng Users.
        /// </summary>
        [Required(ErrorMessage = "Theo dõi phải có người thực hiện")]
        public int UserId { get; set; }

        /// <summary>
        /// Tham chiếu đến đối tượng User.
        /// Người theo dõi.
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// Mã chiến dịch được theo dõi (khóa ngoại).
        /// Liên kết đến bảng Campaigns.
        /// </summary>
        [Required(ErrorMessage = "Theo dõi phải liên kết đến chiến dịch")]
        public int CampaignId { get; set; }

        /// <summary>
        /// Tham chiếu đến đối tượng Campaign.
        /// Chiến dịch được theo dõi.
        /// Khi chiến dịch bị xóa, hành động theo dõi cũng bị xóa (cascade delete).
        /// </summary>
        [ForeignKey("CampaignId")]
        public Campaign Campaign { get; set; }

        /// <summary>
        /// Ngày và giờ người dùng bắt đầu theo dõi chiến dịch.
        /// Được thiết lập tự động bởi hệ thống.
        /// </summary>
        public DateTime FollowedAt { get; set; } = DateTime.Now;
    }
}
