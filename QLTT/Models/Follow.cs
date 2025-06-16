using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTT.Models
{
    public class Follow
    {
        [Key]
        public int FollowId { get; set; } // Mã theo dõi

        [Required]
        public int UserId { get; set; } // Người theo dõi

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public int CampaignId { get; set; } // Chiến dịch được theo dõi

        [ForeignKey("CampaignId")]
        public Campaign Campaign { get; set; }

        public DateTime FollowedAt { get; set; } = DateTime.Now; // Ngày bắt đầu theo dõi
    }
}
