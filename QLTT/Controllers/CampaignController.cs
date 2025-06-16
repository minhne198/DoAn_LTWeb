using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLTT.Data;
using QLTT.Models;
using QLTT.Models.ViewModels;
using System.IO;

namespace QLTT.Controllers
{
    public class CampaignController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CampaignController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Campaign/
        public async Task<IActionResult> Index()
        {
            var campaigns = await _context.Campaigns
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(campaigns);
        }

        // GET: /Campaign/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var campaign = await _context.Campaigns
                .FirstOrDefaultAsync(c => c.CampaignId == id);

            if (campaign == null) return NotFound();

            return View(campaign);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CampaignCreateViewModel vm, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                string imagePath = null;

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Tạo đường dẫn lưu ảnh: wwwroot/uploads
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    // Lưu đường dẫn tương đối vào DB
                    imagePath = "/uploads/" + uniqueFileName;
                }

                var campaign = new Campaign
                {
                    Title = vm.Title,
                    ReceiverInfo = vm.ReceiverInfo,
                    Content = vm.Content,
                    ImagePath = imagePath,
                    TargetAmount = vm.TargetAmount,
                    SuggestedAmount = vm.SuggestedAmount ?? 0,
                    EndDate = vm.EndDate,

                    StartDate = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CurrentAmount = 0,
                    IsApproved = false,
                    IsCompleted = false,
                    UserId = 1 // ⚠️ TODO: Lấy từ session sau
                };

                _context.Campaigns.Add(campaign);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(vm);
        }
    }
}
