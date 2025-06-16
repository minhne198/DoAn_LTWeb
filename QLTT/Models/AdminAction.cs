using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTT.Models
{
    public class AdminAction  // Lưu trữ các hành động của quản trị viên trong hệ thống
    {
        [Key]
        public int AdminActionId { get; set; }

        [Required]
        public int AdminId { get; set; } // Người thực hiện hành động (admin)

        [ForeignKey("AdminId")]
        public User Admin { get; set; }

        public string ActionType { get; set; } // Loại hành động: Duyệt bài, Khóa user...

        public int? TargetId { get; set; } // ID mục tiêu (nếu có)

        public string Notes { get; set; } // Ghi chú hành động

        public DateTime ActionAt { get; set; } = DateTime.Now;
    }
}
