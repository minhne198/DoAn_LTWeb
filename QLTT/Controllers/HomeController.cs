using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QLTT.Models;

namespace QLTT.Controllers
{
    /// <summary>
    /// HomeController - Xử lý các trang chính của ứng dụng.
    /// 
    /// Bao gồm:
    /// - Index: Trang chủ
    /// - Privacy: Chính sách bảo mật
    /// - Error: Trang hiển thị lỗi
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Constructor - Inject Logger.
        /// </summary>
        /// <param name="logger">Logger instance</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// GET: /Home/ hoặc /Home/Index
        /// Hiển thị trang chủ (homepage) của ứng dụng.
        /// 
        /// Trang chủ có thể chứa:
        /// - Thông tin giới thiệu về nền tảng
        /// - Danh sách chiến dịch nổi bật
        /// - Thống kê quyên góp
        /// - Call-to-action (tạo chiến dịch, quyên góp)
        /// </summary>
        /// <returns>View trang chủ</returns>
        public IActionResult Index()
        {
            _logger.LogInformation("Người dùng truy cập trang chủ");
            return View();
        }

        /// <summary>
        /// GET: /Home/Privacy
        /// Hiển thị trang Chính sách bảo mật (Privacy Policy).
        /// 
        /// Nội dung thường bao gồm:
        /// - Cách chúng tôi thu thập dữ liệu
        /// - Cách chúng tôi sử dụng dữ liệu
        /// - Quyền của người dùng
        /// - GDPR compliance thông tin
        /// </summary>
        /// <returns>View trang chính sách bảo mật</returns>
        public IActionResult Privacy()
        {
            _logger.LogInformation("Người dùng truy cập trang chính sách bảo mật");
            return View();
        }

        /// <summary>
        /// GET: /Home/Error
        /// Hiển thị trang lỗi khi có unhandled exception hoặc lỗi HTTP (404, 500, etc.).
        /// 
        /// Cấu hình:
        /// - [ResponseCache]: Không được phép cache trang lỗi
        ///   - Duration: 0 (không cache)
        ///   - Location: None (không cache ở bất kỳ đâu)
        ///   - NoStore: true (không lưu response vào cache)
        /// 
        /// RequestId:
        /// - Activity.Current?.Id: ID của distributed trace (nếu có)
        /// - HttpContext.TraceIdentifier: ID của HTTP trace
        /// - Dùng để tracking lỗi và debug
        /// </summary>
        /// <returns>View trang lỗi với ErrorViewModel</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            try
            {
                // Lấy RequestId để tracking lỗi
                var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
                
                _logger.LogError($"Trang lỗi được hiển thị - RequestId: {requestId}");
                
                // Truyền ErrorViewModel chứa RequestId đến view
                return View(new ErrorViewModel { RequestId = requestId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hiển thị trang lỗi");
                // Trả về generic error response
                return View(new ErrorViewModel { RequestId = "unknown" });
            }
        }
    }
}
