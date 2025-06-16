using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QLTT.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }  // Mã người dùng (khóa chính)

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } // Họ tên người dùng

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } // Email (đăng nhập)

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } // Vai trò: "admin" hoặc "user"

        public bool IsLocked { get; set; } = false; // Có bị khóa tài khoản không?

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Ngày tạo tài khoản

        // Mối quan hệ
        public ICollection<Campaign> Campaigns { get; set; } // Các chiến dịch người này tạo
        public ICollection<Donation> Donations { get; set; } // Các khoản đã quyên góp
    }
}
