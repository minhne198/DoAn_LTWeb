# QLTT - Hệ Thống Quản Lý Từ Thiện
Hệ thống quản lý quyên góp và chiến dịch từ thiện trực tuyến - Personal Life OS (Charity Management Module)

## Công Nghệ Sử Dụng (Tech Stack)
- **Framework**: ASP.NET Core MVC 8.0
- **Ngôn ngữ**: C#
- **Cơ sở dữ liệu**: SQL Server (Entity Framework Core)
- **Giao diện (Styling)**: Bootstrap 5, Custom CSS
- **Thành phần UI**: FontAwesome, jQuery
- **Xác thực**: Cookie-based Authentication

## Tính Năng Chính (Features)
- **Xác thực người dùng** - Đăng nhập/Đăng ký bảo mật với phân quyền Admin/User.
- **Quản lý chiến dịch** - Tạo, xem chi tiết và quản lý các cuộc vận động quyên góp.
- **Quyên góp trực tuyến** - Gửi đóng góp kèm lời nhắn cho từng chiến dịch cụ thể.
- **Tương tác xã hội** - Bình luận và theo dõi (Follow) các chiến dịch quan tâm.
- **Bảng điều khoản Admin** - Duyệt chiến dịch, quản lý người dùng và xem báo cáo.
- **Lịch sử hoạt động** - Lưu trữ Audit Logs và thông báo (Notifications).

## Bắt Đầu (Getting Started)

### Yêu cầu hệ thống (Prerequisites)
- .NET SDK 8.0+
- SQL Server (LocalDB hoặc Remote)

### Cài đặt (Installation)
1. Di chuyển vào thư mục dự án:
   ```bash
   cd QLTT
   ```
2. Khôi phục các gói phụ thuộc:
   ```bash
   dotnet restore
   ```
3. Cập nhật cơ sở dữ liệu (Migrations):
   ```bash
   dotnet ef database update
   ```

### Chạy ứng dụng (Run Development)
Sử dụng lệnh sau để chạy:
```bash
dotnet run
```
Mở trình duyệt tại: `https://localhost:5001` hoặc `http://localhost:5000`

### Biên dịch (Build Production)
```bash
dotnet publish -c Release
```

## Biến Môi Trường (Environment Variables)
Cấu hình trong file `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=QLTT;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

## Cấu Trúc Dự Án (Project Structure)
```text
QLTT/
├── Controllers/         # Xử lý Logic yêu cầu (Campaign, Donation, Account...)
├── Models/              # Lớp dữ liệu và ViewModels
├── Views/               # Giao diện Razor (.cshtml)
│   ├── Campaign/        # Trang danh sách và chi tiết chiến dịch
│   ├── Donation/        # Trang quyên góp
│   ├── Admin/           # Trang quản trị hệ thống
│   └── Shared/          # Layout chung và components
├── Data/                # DbContext và cấu hình Migration
└── wwwroot/             # Tài nguyên tĩnh (CSS, JS, Uploads ảnh)
```

## Các Trang Chính (Pages)

### Đăng nhập (/Account/Login)
- Đăng nhập bằng tài khoản người dùng hoặc admin.
- Chức năng đăng ký tài khoản mới.

### Bảng điều khiển/Trang chủ (/Campaign/Index)
- Hiển thị danh sách các chiến dịch đang hoạt động.
- Sắp xếp theo ngày tạo hoặc mức độ ưu tiên.

### Chi tiết chiến dịch (/Campaign/Details/{id})
- Xem thông tin chi tiết, mục tiêu tài chính, và các khoản đóng góp đã nhận.
- Phần bình luận và danh sách người ủng hộ.

### Quyên góp (/Donation/Donate/{id})
- Form nhập số tiền đóng góp.
- Gửi lời nhắn (Message) kèm theo khoản ủng hộ.

### Quản trị (/Admin/Dashboard)
- Duyệt các chiến dịch mới tạo từ người dùng.
- Quản lý danh sách người dùng và vai trò.
- Xem báo cáo tổng quan về dòng tiền.

### Cá nhân (/Profile/Index)
- Cập nhật thông tin cá nhân.
- Xem lịch sử các khoản đóng góp đã thực hiện.

## License
[MIT License]
