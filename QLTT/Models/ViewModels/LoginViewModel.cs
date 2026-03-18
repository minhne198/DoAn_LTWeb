using System.ComponentModel.DataAnnotations;

namespace QLTT.Models.ViewModels
{
    /// <summary>
    /// ViewModel cho trang đăng nhập (Login page).
    /// 
    /// Mục đích:
    /// - Binding dữ liệu từ form HTML đến action method
    /// - Validation server-side (kiểm tra dữ liệu hợp lệ)
    /// - Display custom labels trên form
    /// 
    /// Quy trình binding:
    /// 1. User nhập email + password vào form HTML
    /// 2. Form POST đến AccountController.Login(LoginViewModel model)
    /// 3. ASP.NET Core tự động bind dữ liệu HTML → ViewModel properties
    /// 4. ModelState.IsValid kiểm tra validation
    /// 5. Nếu có lỗi, hiển thị lại form với error messages
    /// 6. Nếu hợp lệ, xử lý đăng nhập
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Email của người dùng (dùng làm username).
        /// 
        /// Validation:
        /// - [Required]: Email bắt buộc phải nhập
        /// - [EmailAddress]: Email phải đúng định dạng (abc@def.com)
        /// 
        /// Bind từ: <input type="email" name="Email" />
        /// </summary>
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        /// <summary>
        /// Mật khẩu người dùng.
        /// 
        /// Validation:
        /// - [Required]: Mật khẩu bắt buộc phải nhập
        /// - [DataType(DataType.Password)]: Thông báo cho HTML để hiển thị ••••
        /// 
        /// Bind từ: <input type="password" name="Password" />
        /// 
        /// SECURITY:
        /// - Không bao giờ log mật khẩu
        /// - Không bao giờ hiển thị mật khẩu
        /// - Mật khẩu sẽ bị mã hóa trong database
        /// </summary>
        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Checkbox "Ghi nhớ đăng nhập" (Remember Me).
        /// 
        /// Mục đích:
        /// - Nếu true: Cookie sẽ tồn tại sau khi đóng browser
        /// - Nếu false: Cookie chỉ tồn tại trong session (hết khi đóng browser)
        /// 
        /// Bind từ: <input type="checkbox" name="RememberMe" />
        /// </summary>
        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }
}
