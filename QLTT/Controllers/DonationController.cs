using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLTT.Data;
using QLTT.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace QLTT.Controllers
{
    /// <summary>
    /// Controller xử lý tất cả chức năng liên quan đến quyên góp/đóng góp.
    /// 
    /// Quy trình quyên góp:
    /// 1. User chọn chiến dịch cần ủng hộ
    /// 2. GET /Donation/Donate/{campaignId} - Hiển thị form quyên góp
    /// 3. User nhập lượng tiền và lời nhắn
    /// 4. POST /Donation/Create - Xử lý tạo donation mới
    ///    - Kiểm tra campaign tồn tại
    ///    - Kiểm tra user đã đăng nhập
    ///    - Lưu donation vào database
    ///    - Cập nhật CurrentAmount của campaign
    ///    - Chuyển hướng về chi tiết campaign
    /// 
    /// SECURITY & VALIDATION:
    /// - Yêu cầu user phải đăng nhập (check User.Identity)
    /// - Kiểm tra campaign tồn tại trước khi tạo donation
    /// - Validate amount > 0 quá server-side
    /// - CSRF protection với [ValidateAntiForgeryToken]
    /// </summary>
    public class DonationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DonationController> _logger;

        /// <summary>
        /// Constructor - Inject AppDbContext và Logger.
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="logger">Logger instance</param>
        public DonationController(AppDbContext context, ILogger<DonationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// GET: /Donation/Donate/{campaignId}
        /// Hiển thị form quyên góp cho một chiến dịch cụ thể.
        /// 
        /// Quy trình:
        /// 1. Kiểm tra ID campaign hợp lệ
        /// 2. Tìm campaign theo ID
        /// 3. Kiểm tra campaign tồn tại
        /// 4. Tạo object donation với CampaignId được set
        /// 5. Trả về view form với donation object
        /// 
        /// Validation:
        /// - ID campaign phải > 0
        /// - Campaign phải tồn tại trong database
        /// </summary>
        /// <param name="campaignId">ID của chiến dịch cần quyên góp</param>
        /// <returns>View form quyên góp hoặc 404 NotFound</returns>
        [HttpGet]
        public async Task<IActionResult> Donate(int campaignId)
        {
            try
            {
                // Kiểm tra ID hợp lệ
                if (campaignId <= 0)
                {
                    _logger.LogWarning($"ID chiến dịch không hợp lệ: {campaignId}");
                    return NotFound();
                }

                // Tìm campaign theo ID
                var campaign = await _context.Campaigns.FindAsync(campaignId);
                
                // Nếu campaign không tồn tại, trả về 404
                if (campaign == null)
                {
                    _logger.LogWarning($"Không tìm thấy chiến dịch với ID: {campaignId}");
                    return NotFound();
                }

                // Kiểm tra campaign có được duyệt không (tùy chọn)
                // Có thể uncomment nếu muốn chỉ cho phép quyên góp cho campaign đã duyệt
                // if (!campaign.IsApproved)
                // {
                //     _logger.LogWarning($"Cố gắng quyên góp cho campaign chưa được duyệt: {campaignId}");
                //     return NotFound();
                // }

                // Tạo object donation để bind vào form
                var donation = new Donation
                {
                    CampaignId = campaignId
                };

                _logger.LogInformation($"Hiển thị form quyên góp cho campaign ID {campaignId}");
                return View(donation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi hiển thị form quyên góp cho campaign {campaignId}");
                TempData["ErrorMessage"] = "Có lỗi khi tải form quyên góp.";
                return RedirectToAction("Index", "Campaign");
            }
        }

        /// <summary>
        /// POST: /Donation/Create
        /// Xử lý yêu cầu tạo khoản quyên góp mới.
        /// Yêu cầu user phải đăng nhập.
        /// 
        /// Quy trình:
        /// 1. Kiểm tra ModelState (validation)
        /// 2. Tìm campaign theo ID
        /// 3. Kiểm tra campaign tồn tại
        /// 4. Lấy UserId từ claims (user hiện tại đã đăng nhập)
        /// 5. Tạo donation object
        /// 6. Lưu donation vào database
        /// 7. Cập nhật CurrentAmount của campaign
        /// 8. Lưu lại campaign vào database (SaveChanges lần 2)
        /// 9. Chuyển hướng về chi tiết campaign
        /// 
        /// VALIDATION:
        /// - ModelState phải hợp lệ (Amount > 0, etc.)
        /// - Campaign phải tồn tại
        /// - User phải đăng nhập (được kiểm tra bởi [Authorize] middleware)
        /// </summary>
        /// <param name="donation">Object donation từ form (CampaignId, Amount, Message)</param>
        /// <returns>View donation hoặc chuyển hướng đến Campaign/Details</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]  // Yêu cầu user phải đăng nhập
        public async Task<IActionResult> Create([Bind("CampaignId,Amount,Message")] Donation donation)
        {
            try
            {
                // 1. Kiểm tra ModelState
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dữ liệu quyên góp không hợp lệ");
                    return View("Donate", donation);
                }

                // 2. Tìm campaign theo ID
                var campaign = await _context.Campaigns.FindAsync(donation.CampaignId);
                
                // 3. Kiểm tra campaign tồn tại
                if (campaign == null)
                {
                    _logger.LogWarning($"Không tìm thấy campaign với ID: {donation.CampaignId}");
                    ModelState.AddModelError("", "Chiến dịch không tồn tại.");
                    return View("Donate", donation);
                }

                // 4. Lấy UserId từ claims (thông tin user đã đăng nhập)
                var userIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    _logger.LogError("Không thể lấy UserId từ claims");
                    ModelState.AddModelError("", "Có lỗi xác định người dùng.");
                    return View("Donate", donation);
                }

                // 5. Tạo donation object
                // Set các giá trị không từ form (để bảo mật)
                donation.UserId = userId;
                donation.CreatedAt = DateTime.Now;
                donation.Status = "Pending";  // Trạng thái mặc định

                // 6. Lưu donation vào database
                _context.Add(donation);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Tạo donation ID {donation.DonationId} thành công - User: {userId}, Campaign: {donation.CampaignId}, Amount: {donation.Amount}");

                // 7 & 8. Cập nhật CurrentAmount của campaign (tính tổng tiền đã quyên góp)
                // CÁCH 1: Cập nhật trực tiếp (đơn giản)
                campaign.CurrentAmount += donation.Amount;
                _context.Update(campaign);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Cập nhật CurrentAmount campaign {campaign.CampaignId}: {campaign.CurrentAmount}");

                // 9. Chuyển hướng về chi tiết campaign
                TempData["SuccessMessage"] = "Cảm ơn bạn đã ủng hộ chiến dịch! 💝";
                return RedirectToAction("Details", "Campaign", new { id = donation.CampaignId });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Lỗi database khi tạo donation");
                ModelState.AddModelError("", "Có lỗi khi xử lý quyên góp. Vui lòng thử lại sau.");
                return View("Donate", donation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không mong đợi khi tạo donation");
                ModelState.AddModelError("", "Có lỗi hệ thống. Vui lòng thử lại sau.");
                return View("Donate", donation);
            }
        }

        // TODO: Thêm các action sau
        // - GetDonationsByUser: Xem danh sách quyên góp của user hiện tại
        // - GetDonationsByCampaign: Xem danh sách quyên góp cho campaign (với phân trang)
        // - UpdateDonationStatus: Cập nhật trạng thái donation (admin only)
        // - DeleteDonation: Xóa donation (user hoặc admin only)
        // - ReceiptDonation: In hóa đơn/biên nhận donation
    }
}
