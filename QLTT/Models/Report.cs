using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTT.Models
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; } // Mã báo cáo

        [Required]
        public int CampaignId { get; set; }

        [ForeignKey("CampaignId")]
        public Campaign Campaign { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        [MaxLength(255)]
        public string Reason { get; set; } // Lý do báo cáo

        public bool IsHandled { get; set; } = false; // Đã xử lý chưa?

        public DateTime ReportedAt { get; set; } = DateTime.Now; // Thời gian báo cáo
    }
}
