using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLTT.Data;
using QLTT.Models;

namespace QLTT.Controllers
{
    public class DonationController : Controller
    {
        private readonly AppDbContext _context;

        public DonationController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Donate(int campaignId)
        {
            var campaign = await _context.Campaigns.FindAsync(campaignId);
            if (campaign == null)
            {
                return NotFound();
            }

            var donation = new Donation
            {
                CampaignId = campaignId
            };

            return View(donation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CampaignId,Amount,Message")] Donation donation)
        {
            if (ModelState.IsValid)
            {
                var campaign = await _context.Campaigns.FindAsync(donation.CampaignId);
                if (campaign == null)
                {
                    return NotFound();
                }

                donation.UserId = int.Parse(User.Identity.Name); // Lấy ID của user đang đăng nhập
                donation.CreatedAt = DateTime.Now;
                donation.Status = "Pending"; // Trạng thái mặc định là đang chờ xử lý

                _context.Add(donation);
                await _context.SaveChangesAsync();

                // Cập nhật số tiền hiện có của chiến dịch
                campaign.CurrentAmount += donation.Amount;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cảm ơn bạn đã ủng hộ chiến dịch!";
                return RedirectToAction("Details", "Campaign", new { id = donation.CampaignId });
            }

            return View("Donate", donation);
        }
    }
}
