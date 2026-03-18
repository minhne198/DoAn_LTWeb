namespace QLTT.Models
{
    /// <summary>
    /// ErrorViewModel - Model cho trang hiển thị lỗi (Error page).
    /// 
    /// Mục đích:
    /// - Truyền thông tin lỗi từ HomeController.Error() → Error.cshtml view
    /// - Hiển thị RequestId để người dùng cung cấp cho support
    /// - Giúp debugging bằng cách tracking lỗi qua RequestId
    /// 
    /// Quy trình:
    /// 1. Exception xảy ra (unhandled)
    /// 2. Exception handler middleware bắt được
    /// 3. Chuyển hướng đến /Home/Error
    /// 4. HomeController.Error() tạo ErrorViewModel với RequestId
    /// 5. Pass ErrorViewModel đến Error.cshtml view
    /// 6. View hiển thị lỗi + RequestId cho người dùng
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// ID duy nhất của request bị lỗi.
        /// 
        /// Nguồn:
        /// - Activity.Current?.Id: ID của distributed trace (nếu có)
        /// - Hoặc HttpContext.TraceIdentifier: ID của HTTP trace
        /// 
        /// Tác dụng:
        /// - Giúp developer tìm log lỗi tương ứng
        /// - Người dùng có thể cung cấp ID này cho support
        /// - Quá trình debugging được skip đi
        /// 
        /// Nullable (string?) vì lỗi có thể không có RequestId
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Kiểm tra xem có nên hiển thị RequestId không.
        /// 
        /// Logic:
        /// - true: Nếu RequestId không null, không empty, và không whitespace
        /// - false: Nếu RequestId trống
        /// 
        /// Sử dụng trong view:
        /// @if (Model.ShowRequestId)
        /// {
        ///     <p>RequestId: @Model.RequestId</p>
        /// }
        /// 
        /// Điều này tránh hiển thị "RequestId: " khi không có giá trị
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
