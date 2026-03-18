using System.ComponentModel.DataAnnotations;

namespace QLTT.Models.ViewModels
{
    /// <summary>
    /// ViewModel cho trang đăng ký tài khoản (Registration page).
    /// 
    /// Mục đích:
    /// - Binding dữ liệu từ form đăng ký → action method
    /// - Validation dữ liệu đầu vào (email, mật khẩu, etc.)
    /// - Trong quá trình đăng ký, data này sẽ được sử dụng để tạo User entity
    /// 
    /// Quy trình đăng ký:
    /// 1. User điền form (FullName, Email, Password, ConfirmPassword)
    /// 2. Form POST → AccountController.Register(RegisterViewModel model)
    /// 3. ASP.NET Core bind HTML → ViewModel
    /// 4. Server-side validate (kiểm tra email chưa tồn tại, mật khẩu khớp, etc.)
    /// 5. Nếu hợp lệ, tạo User mới và lưu vào database
    /// 6. Chuyển hướng đến trang đăng nhập
    /// 
    /// Lưu ý: Đây là ViewModel (không phải Entity Model)
    /// ViewModel dùng riêng cho form, không lưu trực tiếp vào database
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Họ và tên người dùng.
        /// 
        /// Validation:
        /// - [Required]: Bắt buộc phải nhập
        /// 
        /// Sử dụng: Khi tạo User entity, copy value này → User.FullName
        /// 
        /// Bind từ: <input type="text" name="FullName" />
        /// </summary>
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        public string FullName { get; set; }

        /// <summary>
        /// Email người dùng (dùng làm username duy nhất).
        /// 
        /// Validation:
        /// - [Required]: Bắt buộc phải nhập
        /// - [EmailAddress]: Phải là email hợp lệ (abc@def.com)
        /// 
        /// Server-side thêm:
        /// - Kiểm tra email chưa được sử dụng (tránh duplicate)
        /// - Convert thành lowercase trước khi lưu (để case-insensitive)
        /// 
        /// Sử dụng: Copy → User.Email
        /// 
        /// Bind từ: <input type="email" name="Email" />
        /// </summary>
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        /// <summary>
        /// Mật khẩu tài khoản.
        /// 
        /// Validation:
        /// - [Required]: Bắt buộc phải nhập
        /// - [DataType(DataType.Password)]: Thông báo HTML để ẩn dạng ••••
        /// 
        /// Server-side sẽ:
        /// - Mã hóa mật khẩu bằng PBKDF2 trước khi lưu
        /// - KHÔNG bao giờ lưu mật khẩu plaintext
        /// 
        /// Sử dụng: Mã hóa → User.PasswordHash
        /// 
        /// SECURITY:
        /// - Không in log mật khẩu
        /// - Không gửi mật khẩu qua email (ngoại trừ email forgot password)
        /// 
        /// Bind từ: <input type="password" name="Password" />
        /// </summary>
        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Xác nhận mật khẩu (nhập lại mật khẩu).
        /// 
        /// Validation:
        /// - [Required]: Bắt buộc phải nhập
        /// - [Compare("Password")]: Phải khớp với Password field
        /// - [DataType(DataType.Password)]: Ẩn dạng ••••
        /// 
        /// Mục đích: Tránh user nhập sai mật khẩu do typo
        /// 
        /// Sử dụng: Chỉ dùng cho validation, không lưu vào database
        /// 
        /// Bind từ: <input type="password" name="ConfirmPassword" />
        /// </summary>
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
