using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTT.Models
{
    public class Comment // Bình luận của người dùng về chiến dịch quyên góp
    {
        [Key]
        public int CommentId { get; set; } // Mã bình luận

        [Required]
        public int CampaignId { get; set; } 

        [ForeignKey("CampaignId")]
        public Campaign Campaign { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public string Content { get; set; } // Nội dung bình luận

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
