using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTT.Models
{
    public class AuditLog  // Lưu trữ các hành động của người dùng trong hệ thống
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        public int UserId { get; set; } // Người thực hiện hành động

        [ForeignKey("UserId")]
        public User User { get; set; }

        public string Action { get; set; } // Mô tả hành động

        public DateTime Timestamp { get; set; } = DateTime.Now; // Thời điểm
    }
}
