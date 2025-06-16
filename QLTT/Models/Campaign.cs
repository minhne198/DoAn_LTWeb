using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace QLTT.Models
{
    public class Campaign  // Chiến dịch quyên góp
    {
        [Key]
        public int CampaignId { get; set; } // Mã chiến dịch

        [Required]
        public int UserId { get; set; } // Người tạo chiến dịch

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } // Tiêu đề chiến dịch

        public string ReceiverInfo { get; set; } // Thông tin người nhận hỗ trợ

        public string Content { get; set; } // Nội dung bài đăng

        public string ImagePath { get; set; } // Đường dẫn hình ảnh minh họa

        public decimal TargetAmount { get; set; } // Số tiền cần quyên góp

        public decimal SuggestedAmount { get; set; } // Gợi ý số tiền mỗi người

        public decimal CurrentAmount { get; set; } = 0; // Số tiền đã nhận

        //public string ProofDocumentPath { get; set; } // Tài liệu minh chứng

        public bool IsApproved { get; set; } = false; // Đã được duyệt chưa?

        public bool IsCompleted { get; set; } = false; // Đã hoàn thành chưa?

        public DateTime? StartDate { get; set; } // Ngày bắt đầu

        public DateTime? EndDate { get; set; } // Ngày kết thúc

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Mối quan hệ
        public ICollection<Donation> Donations { get; set; } // Danh sách người ủng hộ
        public ICollection<Comment> Comments { get; set; } // Danh sách bình luận
    }
}
