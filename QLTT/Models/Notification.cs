using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTT.Models
{
    public class Notification  // Thông báo cho người dùng về các sự kiện liên quan đến chiến dịch quyên góp
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public int UserId { get; set; } // Người nhận thông báo

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public string Content { get; set; } // Nội dung thông báo

        public bool IsRead { get; set; } = false; // Đã đọc chưa?

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Ngày tạo thông báo
    }
}
