using Microsoft.AspNetCore.Mvc;
using QLTT.Data;
using QLTT.Models;
using QLTT.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;
using System.Text;

namespace QLTT.Controllers
{
    /// <summary>
    /// Controller xử lý tất cả các chức năng liên quan đến tài khoản người dùng.
    /// Bao gồm: đăng ký, đăng nhập, đăng xuất.
    /// 
    /// SECURITY NOTES:
    /// - Mật khẩu được mã hóa sử dụng PBKDF2 (Password-Based Key Derivation Function 2)
    /// - Sử dụng salting để tăng cường bảo mật
    /// - Khuyến nghị: Nâng cấp lên BCrypt hoặc Argon2 cho bảo mật tốt hơn
    /// - Tài khoản bị khóa sẽ không thể đăng nhập
    /// </summary>
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AccountController> _logger;

        /// <summary>
        /// Constructor - Inject AppDbContext và Logger.
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="logger">Logger instance</param>
        public AccountController(AppDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// GET: /Account/Register
        /// Hiển thị form đăng ký tài khoản mới.
        /// </summary>
        /// <returns>View form đăng ký</returns>
        [HttpGet]
        public IActionResult Register()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                // Log exception để debuggging
                _logger.LogError(ex, "Lỗi khi hiển thị form đăng ký");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// POST: /Account/Register
        /// Xử lý yêu cầu đăng ký tài khoản mới.
        /// 
        /// Quy trình:
        /// 1. Kiểm tra ModelState (validation)
        /// 2. Kiểm tra email đã tồn tại
        /// 3. Mã hóa mật khẩu sử dụng PBKDF2
        /// 4. Lưu user mới vào database
        /// 5. Chuyển hướng đến trang đăng nhập
        /// </summary>
        /// <param name="model">Dữ liệu form đăng ký (FullName, Email, Password, ConfirmPassword)</param>
        /// <returns>View với lỗi hoặc chuyển hướng đến Login</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            try
            {
                // 1. Kiểm tra tính hợp lệ của dữ liệu đầu vào
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dữ liệu đăng ký không hợp lệ");
                    return View(model);
                }

                // 2. Kiểm tra email đã được sử dụng chưa
                var userExists = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (userExists != null)
                {
                    // Email đã tồn tại - thêm lỗi vào ModelState và hiển thị lại form
                    ModelState.AddModelError("Email", "Email đã được sử dụng. Vui lòng chọn email khác.");
                    _logger.LogWarning($"Cố gắng đăng ký với email đã tồn tại: {model.Email}");
                    return View(model);
                }

                // 3. Mã hóa mật khẩu sử dụng PBKDF2 (Password-Based Key Derivation Function 2)
                // Công thức: PBKDF2 = Hash(Password + Salt) với 10,000 iterations
                // Mục đích: Làm cho việc crack mật khẩu trở nên khó khăn hơn
                string passwordHash = HashPassword(model.Password);

                // 4. Tạo user mới
                var newUser = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = passwordHash,  // Lưu hash, KHÔNG lưu mật khẩu gốc
                    Role = "user",                 // Mặc định role là người dùng thường
                    IsLocked = false,              // Tài khoản mới không bị khóa
                    CreatedAt = DateTime.Now
                };

                // 5. Lưu user vào database
                _context.Users.Add(newUser);
                _context.SaveChanges();

                _logger.LogInformation($"Tài khoản mới được tạo: {model.Email}");

                // 6. Chuyển hướng đến trang đăng nhập
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (DbUpdateException dbEx)
            {
                // Lỗi database
                _logger.LogError(dbEx, "Lỗi database khi đăng ký user");
                ModelState.AddModelError("", "Có lỗi khi lưu dữ liệu. Vui lòng thử lại sau.");
                return View(model);
            }
            catch (Exception ex)
            {
                // Lỗi không mong đợi
                _logger.LogError(ex, "Lỗi không mong đợi khi đăng ký user");
                ModelState.AddModelError("", "Có lỗi hệ thống. Vui lòng thử lại sau.");
                return View(model);
            }
        }

        /// <summary>
        /// GET: /Account/Login
        /// Hiển thị form đăng nhập.
        /// </summary>
        /// <returns>View form đăng nhập</returns>
        [HttpGet]
        public IActionResult Login()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hiển thị form đăng nhập");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// POST: /Account/Login
        /// Xử lý yêu cầu đăng nhập.
        /// 
        /// Quy trình:
        /// 1. Kiểm tra ModelState
        /// 2. Tìm kiếm user theo email
        /// 3. So sánh mật khẩu nhập vào với password hash lưu trữ
        /// 4. Kiểm tra tài khoản có bị khóa không
        /// 5. Tạo cookie authentication
        /// 6. Chuyển hướng đến trang campaign
        /// </summary>
        /// <param name="model">Dữ liệu form đăng nhập (Email, Password, RememberMe)</param>
        /// <returns>View với lỗi hoặc chuyển hướng đến Campaign/Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                // 1. Kiểm tra tính hợp lệ của dữ liệu đầu vào
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dữ liệu đăng nhập không hợp lệ");
                    return View(model);
                }

                // 2. Tìm kiếm user theo email
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

                // 3. Kiểm tra user có tồn tại và mật khẩu có đúng không
                if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
                {
                    // Ghi log để phát hiện các cuộc tấn công brute-force
                    _logger.LogWarning($"Nỗi thất bại đăng nhập với email: {model.Email}");
                    
                    // Không cung cấp thông tin cụ thể (email không tồn tại hay mật khẩu sai)
                    // để tăng cường bảo mật, chống lại username enumeration attack
                    ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                    return View(model);
                }

                // 4. Kiểm tra tài khoản có bị khóa không
                if (user.IsLocked)
                {
                    _logger.LogWarning($"Cố gắng đăng nhập với tài khoản bị khóa: {model.Email}");
                    ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
                    return View(model);
                }

                // 5. Tạo claims (thông tin người dùng) để lưu vào cookie
                var claims = new List<Claim>
                {
                    // ClaimTypes.Name được sử dụng làm định danh chính của user
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),  // Dùng để kiểm tra quyền truy cập
                    new Claim("FullName", user.FullName)     // Claim custom cho tên hiển thị
                };

                // Tạo ClaimsIdentity dùng cookie authentication scheme
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                
                // Cấu hình thuộc tính authentication
                var authProperties = new AuthenticationProperties
                {
                    // IsPersistent = true nếu user chọn \"Remember me\"
                    // Có nghĩa là cookie sẽ tồn tại sau khi đóng trình duyệt
                    IsPersistent = model.RememberMe,
                    
                    // ExpiresUtc được thiết lập bởi cấu hình trong Program.cs (30 phút)
                };

                // 6. Ký vào (SignIn) - tạo cookie authentication
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation($"Đăng nhập thành công: {user.Email}");

                // 7. Chuyển hướng đến trang campaign
                return RedirectToAction("Index", "Campaign");
            }
            catch (Exception ex)
            {
                // Lỗi không mong đợi
                _logger.LogError(ex, \"Lỗi không mong đợi khi đăng nhập\");
                ModelState.AddModelError("", "Có lỗi hệ thống. Vui lòng thử lại sau.");
                return View(model);
            }
        }

        /// <summary>
        /// POST: /Account/Logout
        /// Xử lý yêu cầu đăng xuất.
        /// Xóa cookie authentication và chuyển hướng về trang đăng nhập.
        /// </summary>
        /// <returns>Chuyển hướng đến Login</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Lấy thông tin user hiện tại trước khi đăng xuất
                var userId = User.FindFirst(ClaimTypes.Name)?.Value;
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                // Ký xuất (SignOut) - xóa cookie authentication
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                _logger.LogInformation($"Đăng xuất thành công: {userEmail}");

                TempData["SuccessMessage"] = "Đăng xuất thành công!";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng vẫn chuyển hướng để tránh UI bị lỗi
                _logger.LogError(ex, "Lỗi khi đăng xuất");
                return RedirectToAction("Login");
            }
        }

        // ==================== Helper Methods (Phương thức hỗ trợ) ====================

        /// <summary>
        /// Mã hóa mật khẩu bằng thuật toán PBKDF2 (Password-Based Key Derivation Function 2).
        /// 
        /// Cách hoạt động:
        /// 1. Tạo random salt (32 bytes)
        /// 2. Sử dụng PBKDF2 với:
        ///    - Password: mật khẩu gốc
        ///    - Salt: giá trị random
        ///    - Iterations: 10,000 lần (càng nhiều = càng an toàn nhưng chậm hơn)
        ///    - Hash size: 32 bytes
        /// 3. Kết hợp salt + hash thành một chuỗi duy nhất
        /// 4. Trả về chuỗi base64 để lưu vào database
        /// 
        /// Lợi ích:
        /// - Mỗi user có salt khác nhau, ngăn chặn rainbow table attacks
        /// - Iterations cao làm chậm brute force attacks
        /// - PBKDF2 là chuẩn công nghiệp được NIST khuyên cáo
        /// 
        /// TODO: Nâng cấp lên BCrypt hoặc Argon2 để bảo mật tốt hơn
        /// Cài đặt: dotnet add package BCrypt.Net-Core
        /// </summary>
        /// <param name="password">Mật khẩu gốc (plaintext) từ người dùng</param>
        /// <returns>Hash password dưới dạng Base64 string (string kết hợp salt + hash)</returns>
        private string HashPassword(string password)
        {
            try
            {
                // 1. Tạo random salt (32 bytes = 256 bits) để tăng độ bảo mật
                using (var rng = new RNGCryptoServiceProvider())
                {
                    byte[] salt = new byte[32];
                    rng.GetBytes(salt);  // Sinh ngẫu nhiên các bytes

                    // 2. Tạo Rfc2898DeriveBytes (PBKDF2) với:
                    //    - password: mật khẩu gốc
                    //    - salt: giá trị random vừa tạo
                    //    - iterations: 10,000 lần (mặc định của ntiệu chuẩn)
                    //    Iterations cao = chậm hơn nhưng an toàn hơn
                    using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
                    {
                        // 3. Lấy hash (32 bytes)
                        byte[] hash = pbkdf2.GetBytes(32);

                        // 4. Kết hợp salt + hash thành một mảng byte
                        byte[] hashWithSalt = new byte[salt.Length + hash.Length];
                        Array.Copy(salt, 0, hashWithSalt, 0, salt.Length);      // Copy salt vào phần đầu
                        Array.Copy(hash, 0, hashWithSalt, salt.Length, hash.Length);  // Copy hash vào phần sau

                        // 5. Chuyển đổi thành Base64 string để lưu vào database (dễ quản lý)
                        return Convert.ToBase64String(hashWithSalt);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi mã hóa mật khẩu");
                throw;
            }
        }

        /// <summary>
        /// Kiểm tra xem mật khẩu nhập vào có khớp với hash đã lưu không.
        /// 
        /// Cách hoạt động:
        /// 1. Giải mã Base64 hash lưu trữ thành mảng byte
        /// 2. Tách salt (32 bytes đầu) từ hash
        /// 3. Dùng salt này và mật khẩu nhập vào để tạo hash mới (PBKDF2)
        /// 4. So sánh hash mới với hash lưu trữ
        /// 5. Trả về true nếu khớp, false nếu không
        /// 
        /// Lý do sử dụng salt:
        /// - Nếu không có salt, 2 user có cùng mật khẩu sẽ có cùng hash
        /// - Điều này cho phép hacker biết được mật khẩu thông dụng
        /// - Với salt ngẫu nhiên, cùng mật khẩu cũng cho hash khác nhau
        /// </summary>
        /// <param name="enteredPassword">Mật khẩu người dùng nhập vào</param>
        /// <param name="storedHash">Hash mật khẩu đã lưu trong database</param>
        /// <returns>true nếu mật khẩu đúng, false nếu sai</returns>
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            try
            {
                // 1. Giải mã Base64 hash lưu trữ thành mảng byte
                byte[] hashWithSalt = Convert.FromBase64String(storedHash);

                // 2. Tách salt từ hash
                // Salt là 32 bytes đầu tiên
                byte[] salt = new byte[32];
                Array.Copy(hashWithSalt, 0, salt, 0, 32);

                // Hash là các bytes còn lại sau salt
                byte[] storedHashBytes = new byte[hashWithSalt.Length - 32];
                Array.Copy(hashWithSalt, 32, storedHashBytes, 0, hashWithSalt.Length - 32);

                // 3. Tạo hash mới từ mật khẩu nhập vào sử dụng salt đã lưu
                // Điều này rất quan trọng: phải dùng salt cũ để sinh hash mới
                // Khi đó hash mới mới có thể so sánh với hash cũ
                using (var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000, HashAlgorithmName.SHA256))
                {
                    byte[] hashOfInput = pbkdf2.GetBytes(32);

                    // 4. So sánh hash mới với hash cũ
                    // Sử dụng ConstantTimeEquals để chống timing attacks
                    // (nhằm ngăn chặn việc so sánh thời gian lộ thông tin)
                    return CryptographicEquals(hashOfInput, storedHashBytes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác minh mật khẩu");
                return false;  // Trả về false nếu có lỗi (an toàn hơn)
            }
        }

        /// <summary>
        /// So sánh hai byte arrays với constant-time để chống timing attack.
        /// 
        /// Lý do: Nếu dùng == bình thường, thời gian so sánh sẽ tỉ lệ với số byte khớp.
        /// Hacker có thể đoán mật khẩu dựa vào thời gian phản hồi.
        /// 
        /// Hàm này luôn kiểm tra TẤT CẢ bytes dù đã tìm thấy sự khác biệt.
        /// Điều này làm cho thời gian kiểm tra gần như nhất quán.
        /// </summary>
        /// <param name="hash1">Mảng byte thứ nhất</param>
        /// <param name="hash2">Mảng byte thứ hai</param>
        /// <returns>true nếu hai mảng bằng nhau, false nếu khác</returns>
        private bool CryptographicEquals(byte[] hash1, byte[] hash2)
        {
            // Nếu độ dài khác nhau, không thể bằng nhau
            if (hash1.Length != hash2.Length)
                return false;

            // So sánh từng byte một, không dừng sớm ngay cả khi tìm thấy khác biệt
            bool isEqual = true;
            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                    isEqual = false;  // Không dùng break để ngăn timing attack
            }

            return isEqual;
        }
    }
}
