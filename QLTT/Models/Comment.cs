using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTT.Models
{
    /// <summary>
    /// Đại diện cho một bình luận trên chiến dịch quyên góp.
    /// Người dùng có thể bình luận, chia sẻ ý kiến hoặc lần lối động viên 
    /// cho chiến dịch hoặc người cần giúp.
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Mã bình luận duy nhất (khóa chính).
        /// Được tự động sinh bởi database.
        /// </summary>
        [Key]
        public int CommentId { get; set; }

        /// <summary>
        /// Mã chiến dịch mà bình luận này dành cho (khóa ngoại).
        /// Liên kết đến bảng Campaigns.
        /// Bắt buộc phải có.
        /// </summary>
        [Required(ErrorMessage = "Bình luận phải liên kết đến một chiến dịch")]
        public int CampaignId { get; set; }

        /// <summary>
        /// Tham chiếu đến đối tượng Campaign.
        /// Khi chiến dịch bị xóa, tất cả bình luận của nó cũng bị xóa (cascade delete).
        /// </summary>
        [ForeignKey("CampaignId")]
        public Campaign Campaign { get; set; }

        /// <summary>
        /// Mã người dùng tác giả của bình luận (khóa ngoại).
        /// Liên kết đến bảng Users.
        /// Chỉ được xóa khi user yêu cầu nếu không vi phạm quy tắc (restrict delete).
        /// </summary>
        [Required(ErrorMessage = "Bình luận phải có người tác giả")]
        public int UserId { get; set; }

        /// <summary>
        /// Tham chiếu đến đối tượng User.
        /// Người viết bình luận.
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// Nội dung của bình luận.
        /// Bắt buộc điền, tối đa 1000 ký tự.
        /// Ví dụ: "Hy vọng em sớm khỏe mạnh, cộng đồng luôn sát cánh cùng em!"
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập nội dung bình luận")]
        [MaxLength(1000, ErrorMessage = "Bình luận không vượt quá 1000 ký tự")]
        public string Content { get; set; }

        /// <summary>
        /// Ngày và giờ bình luận được tạo.
        /// Được thiết lập tự động bởi hệ thống.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
