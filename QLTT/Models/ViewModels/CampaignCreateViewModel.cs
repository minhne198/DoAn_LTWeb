// CampaignCreateViewModel.cs
using Microsoft.AspNetCore.Http; // THÊM
using System.ComponentModel.DataAnnotations;

namespace QLTT.Models.ViewModels
{
    public class CampaignCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề chiến dịch")]
        [MaxLength(200)]
        public string Title { get; set; }

        public string ReceiverInfo { get; set; }

        public string Content { get; set; }

        [Display(Name = "Ảnh minh họa")]
        public IFormFile ImageFile { get; set; }  // THAY THẾ ImagePath

        [Required(ErrorMessage = "Vui lòng nhập số tiền cần kêu gọi")]
        [Range(1000, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 1.000 VNĐ")]
        public decimal TargetAmount { get; set; }

        [Display(Name = "Mức ủng hộ gợi ý (VNĐ)")]
        public decimal? SuggestedAmount { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}
