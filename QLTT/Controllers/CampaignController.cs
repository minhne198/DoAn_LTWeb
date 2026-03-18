using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLTT.Data;
using QLTT.Models;
using QLTT.Models.ViewModels;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace QLTT.Controllers
{
    /// <summary>
    /// Controller xử lý tất cả các chức năng liên quan đến chiến dịch quyên góp.
    /// Bao gồm: liệt kê, xem chi tiết, tạo chiến dịch.
    /// 
    /// FLOW CHÍNH:
    /// 1. GET /Campaign/ - Hiển thị danh sách tất cả chiến dịch (phân trang theo ngày tạo)
    /// 2. GET /Campaign/Details/{id} - Xem chi tiết một chiến dịch
    /// 3. GET /Campaign/Create - Hiển thị form tạo chiến dịch (require authentication)
    /// 4. POST /Campaign/Create - Xử lý tạo chiến dịch mới (upload ảnh, lưu DB)
    /// </summary>
    public class CampaignController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CampaignController> _logger;

        // Cấu hình cho upload file
        private const long MAX_FILE_SIZE = 5 * 1024 * 1024;  // 5MB
        private static readonly string[] ALLOWED_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".gif" };

        /// <summary>
        /// Constructor - Inject AppDbContext, IWebHostEnvironment, và Logger.
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="env">Web hosting environment (để lấy wwwroot path)</param>
        /// <param name="logger">Logger instance</param>
        public CampaignController(AppDbContext context, IWebHostEnvironment env, ILogger<CampaignController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// GET: /Campaign/ hoặc /Campaign/Index
        /// Hiển thị danh sách TẤT CẢ chiến dịch (không có phân trang, có sắp xếp).
        /// 
        /// Quy trình:
        /// 1. Lấy tất cả campaigns từ database
        /// 2. Sắp xếp theo ngày tạo giảm dần (mới nhất trước)
        /// 3. Trả về cho view để hiển thị dưới dạng danh sách
        /// 
        /// TODO: Thêm phân trang (pagination) khi số campaign tăng lên
        /// </summary>
        /// <returns>Danh sách campaigns</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Lấy tất cả campaigns từ database, sắp xếp theo ngày tạo (mới nhất trước)
                var campaigns = await _context.Campaigns
                    .AsNoTracking()  // AsNoTracking() để cải thiện hiệu năng (chỉ đọc)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                _logger.LogInformation($"Lấy danh sách {campaigns.Count} chiến dịch thành công");
                return View(campaigns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách chiến dịch");
                TempData["ErrorMessage"] = "Có lỗi khi tải danh sách chiến dịch.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// GET: /Campaign/Details/{id}
        /// Hiển thị chi tiết một chiến dịch theo ID.
        /// 
        /// Quy trình:
        /// 1. Kiểm tra ID có hợp lệ không
        /// 2. Tìm kiếm campaign theo ID
        /// 3. Nếu không tìm thấy, trả về 404 (NotFound)
        /// 4. Nếu tìm thấy, trả về view chi tiết
        /// 
        /// TODO: Thêm danh sách donations, comments, followers cho campaign này
        /// </summary>
        /// <param name="id">ID của chiến dịch cần xem</param>
        /// <returns>View chi tiết campaign hoặc 404 NotFound</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Kiểm tra ID hợp lệ
                if (id <= 0)
                {
                    _logger.LogWarning($"ID chiến dịch không hợp lệ: {id}");
                    return NotFound();
                }

                // Tìm campaign theo ID
                var campaign = await _context.Campaigns
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CampaignId == id);

                // Nếu không tìm thấy, trả về 404
                if (campaign == null)
                {
                    _logger.LogWarning($"Không tìm thấy chiến dịch với ID: {id}");
                    return NotFound();
                }

                _logger.LogInformation($"Lấy chi tiết chiến dịch ID {id} thành công");
                return View(campaign);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy chi tiết chiến dịch ID {id}");
                TempData["ErrorMessage"] = "Có lỗi khi tải thông tin chiến dịch.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// GET: /Campaign/Create
        /// Hiển thị form tạo chiến dịch mới.
        /// Yêu cầu user phải đăng nhập ([Authorize] attribute sẽ được thêm vào Program.cs).
        /// </summary>
        /// <returns>View form tạo chiến dịch</returns>
        [HttpGet]
        [Authorize]  // Chỉ người dùng đã đăng nhập mới có thể tạo chiến dịch
        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hiển thị form tạo chiến dịch");
                TempData["ErrorMessage"] = "Có lỗi khi tải form tạo chiến dịch.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// POST: /Campaign/Create
        /// Xử lý yêu cầu tạo chiến dịch mới.
        /// 
        /// Quy trình:
        /// 1. Kiểm tra ModelState (validation)
        /// 2. Lấy UserId từ claims (user hiện tại)
        /// 3. Xử lý upload ảnh (nếu có)
        ///    - Kiểm tra định dạng file
        ///    - Kiểm tra kích thước file
        ///    - Lưu file vào wwwroot/uploads/
        ///    - Lưu đường dẫn vào database
        /// 4. Tạo campaign mới
        /// 5. Lưu vào database
        /// 6. Chuyển hướng đến danh sách chiến dịch
        /// 
        /// SECURITY:
        /// - Kiểm tra kích thước file để chống DoS attack
        /// - Kiểm tra định dạng file
        /// - Sử dụng random GUID làm tên file (chống path traversal)
        /// - Validate all user input (ModelState)
        /// </summary>
        /// <param name="vm">Dữ liệu form tạo chiến dịch</param>
        /// <param name="ImageFile">File ảnh upload (optional)</param>
        /// <returns>View với lỗi hoặc chuyển hướng đến Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CampaignCreateViewModel vm, IFormFile ImageFile)
        {
            try
            {
                // 1. Kiểm tra ModelState (validation từ ViewModel)
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dữ liệu tạo chiến dịch không hợp lệ");
                    return View(vm);
                }

                // 2. Lấy UserId từ claims (thông tin người dùng hiện tại)
                var userIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    _logger.LogError("Không thể lấy UserId từ claims");
                    ModelState.AddModelError("", "Có lỗi khi xác định người dùng.");
                    return View(vm);
                }

                string imagePath = null;

                // 3. Xử lý upload ảnh (nếu có)
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Kiểm tra kích thước file (tối đa 5MB)
                    if (ImageFile.Length > MAX_FILE_SIZE)
                    {
                        ModelState.AddModelError("ImageFile", "Kích thước ảnh không được vượt quá 5MB.");
                        _logger.LogWarning($"File quá lớn: {ImageFile.Length} bytes");
                        return View(vm);
                    }

                    // Kiểm tra định dạng file
                    var fileExtension = Path.GetExtension(ImageFile.FileName).ToLower();
                    if (!ALLOWED_EXTENSIONS.Contains(fileExtension))
                    {
                        ModelState.AddModelError("ImageFile", "Chỉ chấp nhận file ảnh (.jpg, .jpeg, .png, .gif).");
                        _logger.LogWarning($"Định dạng file không hợp lệ: {fileExtension}");
                        return View(vm);
                    }

                    try
                    {
                        // Tạo đường dẫn thư mục uploads: wwwroot/uploads/
                        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                        
                        // Tạo thư mục nếu chưa tồn tại
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                            _logger.LogInformation("Tạo thư mục uploads thành công");
                        }

                        // Tạo tên file duy nhất sử dụng GUID (ngăn chặn path traversal attack)
                        // Ví dụ: a1b2c3d4-e5f6-11ec.jpg
                        string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Lưu file vào disk
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        // Lưu đường dẫn tương đối (relative path) vào database
                        // Điều này cho phép dễ dàng di chuyển ứng dụng mà không cần cập nhật DB
                        imagePath = "/uploads/" + uniqueFileName;
                        _logger.LogInformation($"Upload ảnh thành công: {imagePath}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi khi upload ảnh");
                        ModelState.AddModelError("ImageFile", "Có lỗi khi lưu ảnh. Vui lòng thử lại.");
                        return View(vm);
                    }
                }

                // 4. Tạo campaign mới
                var campaign = new Campaign
                {
                    // Thông tin cơ bản từ form
                    Title = vm.Title.Trim(),
                    ReceiverInfo = vm.ReceiverInfo?.Trim(),
                    Content = vm.Content?.Trim(),
                    ImagePath = imagePath,
                    
                    // Thông tin tài chính
                    TargetAmount = vm.TargetAmount,
                    SuggestedAmount = vm.SuggestedAmount ?? 0,
                    CurrentAmount = 0,  // Mới tạo nên chưa có donation
                    
                    // Thông tin trạng thái
                    IsApproved = false,  // Admin phải duyệt trước
                    IsCompleted = false,
                    
                    // Thông tin thời gian
                    StartDate = DateTime.Now,
                    EndDate = vm.EndDate,
                    CreatedAt = DateTime.Now,
                    
                    // Người tạo chiến dịch
                    UserId = userId
                };

                // 5. Lưu vào database
                _context.Campaigns.Add(campaign);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Tạo chiến dịch mới (ID: {campaign.CampaignId}) thành công - User: {userId}");

                // 6. Chuyển hướng và hiển thị thông báo
                TempData["SuccessMessage"] = "Tạo chiến dịch thành công! Chờ admin duyệt để công khai.";
                return RedirectToAction("Index");
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Lỗi database khi tạo chiến dịch");
                ModelState.AddModelError("", "Có lỗi khi lưu dữ liệu. Vui lòng thử lại sau.");
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không mong đợi khi tạo chiến dịch");
                ModelState.AddModelError("", "Có lỗi hệ thống. Vui lòng thử lại sau.");
                return View(vm);
            }
        }

        // TODO: Thêm các action sau
        // - Edit: Chỉnh sửa chiến dịch (chỉ chủ chiến dịch hoặc admin)
        // - Delete: Xóa chiến dịch (chỉ chủ chiến dịch hoặc admin)
        // - MyCampaigns: Xem danh sách chiến dịch của người dùng hiện tại
        // - Approve: Duyệt chiến dịch (chỉ admin)
        // - Complete: Đánh dấu hoàn thành (chỉ chủ chiến dịch hoặc admin)
    }
}
