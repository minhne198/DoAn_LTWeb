using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace QLTT.Models.ViewModels
{
    /// <summary>
    /// ViewModel cho trang tạo chiến dịch quyên góp (Campaign Create page).
    /// 
    /// Mục đích:
    /// - Binding dữ liệu từ form tạo chiến dịch → action method
    /// - Xử lý upload file ảnh (IFormFile)
    /// - Validation dữ liệu chiến dịch
    /// 
    /// Quy trình tạo chiến dịch:
    /// 1. User điền form (Title, Content, TargetAmount, ảnh, etc.)
    /// 2. Form POST (multipart/form-data) → CampaignController.Create()
    /// 3. ASP.NET Core bind dữ liệu → CampaignCreateViewModel
    /// 4. Server validate
    /// 5. Xử lý upload ảnh (lưu file, lấy path)
    /// 6. Tạo Campaign entity từ ViewModel
    /// 7. Lưu Campaign vào database
    /// 
    /// Lưu ý:
    /// - Đây là ViewModel (không phải Database Model)
    /// - IFormFile dùng để upload file, không lưu trực tiếp vào DB
    /// - ImagePath được set sau cùng khi lưu file thành công
    /// </summary>
    public class CampaignCreateViewModel
    {
        /// <summary>
        /// Tiêu đề/tên chiến dịch.
        /// 
        /// Validation:
        /// - [Required]: Bắt buộc phải nhập
        /// - [MaxLength(200)]: Tối đa 200 ký tự
        /// 
        /// Ví dụ: "Giúp em Lan mua xe lăn"
        /// 
        /// Sử dụng: Copy → Campaign.Title
        /// 
        /// Bind từ: <input type="text" name="Title" />
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề chiến dịch")]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// Thông tin về người/gia đình cần nhận hỗ trợ.
        /// Không bắt buộc nhập.
        /// 
        /// Ví dụ: "Gia đình Nguyễn Văn A tại xã B, huyện C, tỉnh D"
        /// 
        /// Sử dụng: Copy → Campaign.ReceiverInfo
        /// 
        /// Bind từ: <textarea name="ReceiverInfo"></textarea>
        /// </summary>
        public string ReceiverInfo { get; set; }

        /// <summary>
        /// Nội dung chi tiết của chiến dịch.
        /// Mô tả tình hình, lý do kêu gọi, kế hoạch sử dụng tiền, etc.
        /// Không bắt buộc nhập.
        /// 
        /// Có thể chứa HTML formatting (nếu view sử dụng rich text editor)
        /// 
        /// Sử dụng: Copy → Campaign.Content
        /// 
        /// Bind từ: <textarea name="Content"></textarea> (hoặc rich editor)
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// File ảnh minh họa đại diện cho chiến dịch.
        /// 
        /// IFormFile:
        /// - Đại diện file upload từ form (content-type: multipart/form-data)
        /// - Có property: FileName, ContentType, Length, OpenReadStream()
        /// - Không được lưu trực tiếp vào DB
        /// 
        /// Server-side xử lý:
        /// - Kiểm tra kích thước file (max 5MB)
        /// - Kiểm tra định dạng (.jpg, .png, .gif)
        /// - Lưu file vào wwwroot/uploads/ folder
        /// - Tạo tên file duy nhất (GUID + extension)
        /// - Lưu đường dẫn vào Campaign.ImagePath
        /// 
        /// Không bắt buộc upload (có thể null)
        /// 
        /// Bind từ: <input type="file" name="ImageFile" accept="image/*" />
        /// 
        /// Security:
        /// - Validate file size & format để chống malware
        /// - Rename file thành GUID để chống path traversal attack
        /// </summary>
        [Display(Name = "Ảnh minh họa")]
        public IFormFile ImageFile { get; set; }

        /// <summary>
        /// Mục tiêu tài chính - số tiền cần quyên góp (tính bằng VND).
        /// 
        /// Validation:
        /// - [Required]: Bắt buộc phải nhập
        /// - [Range(1000, double.MaxValue)]: Phải lớn hơn 1.000 VNĐ
        /// 
        /// Ví dụ: 5000000 (5 triệu đồng)
        /// 
        /// Sử dụng: Copy → Campaign.TargetAmount
        /// 
        /// Bind từ: <input type="number" name="TargetAmount" step="1000" />
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập số tiền cần kêu gọi")]
        [Range(1000, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 1.000 VNĐ")]
        public decimal TargetAmount { get; set; }

        /// <summary>
        /// Mức ủng hộ gợi ý cho mỗi đơn quyên góp (tính bằng VND).
        /// Không bắt buộc nhập (nullable).
        /// 
        /// Ví dụ: 100000 (100 ngàn đồng)
        /// 
        /// Sử dụng: Copy → Campaign.SuggestedAmount (nếu null, set = 0)
        /// 
        /// Bind từ: <input type="number" name="SuggestedAmount" nullable="true" />
        /// </summary>
        [Display(Name = "Mức ủng hộ gợi ý (VNĐ)")]
        public decimal? SuggestedAmount { get; set; }

        /// <summary>
        /// Ngày kết thúc chiến dịch.
        /// 
        /// Validation:
        /// - [Required]: Bắt buộc phải chọn ngày
        /// - [DataType(DataType.Date)]: Định dạng là ngày (YYYY-MM-DD)
        /// 
        /// Sử dụng: Copy → Campaign.EndDate
        /// 
        /// Lưu ý:
        /// - Ngày bắt đầu được set = DateTime.Now (trong controller)
        /// - Ngày kết thúc nên > ngày bắt đầu
        /// 
        /// Bind từ: <input type="date" name="EndDate" />
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}
