using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTT.Models
{
    public class Donation  // Đóng góp của người dùng cho chiến dịch quyên góp
    {
        [Key]
        public int DonationId { get; set; } // Mã đóng góp

        [Required]
        public int CampaignId { get; set; }

        [ForeignKey("CampaignId")]
        public Campaign Campaign { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public decimal Amount { get; set; } // Số tiền đóng góp

        public string Message { get; set; } // Lời nhắn

        public string Status { get; set; } = "Pending"; // Trạng thái đóng góp

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Ngày tạo

        public DateTime DonatedAt { get; set; } = DateTime.Now; // Ngày đóng góp
    }
}
