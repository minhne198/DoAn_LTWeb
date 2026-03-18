🧠 QLTT – Nền tảng kêu gọi quyên góp từ thiện
🚀 Đồ án web (ASP.NET Core MVC)
Đề tài: Xây dựng hệ thống kêu gọi – tiếp nhận quyên góp minh bạch cho cộng đồng.
Công nghệ: ASP.NET Core MVC · EF Core · SQL Server · Bootstrap · Cookie Auth

📚 Giới thiệu đề tài
Hệ thống cho phép người dùng tạo chiến dịch kêu gọi, theo dõi tiến độ, và ủng hộ trực tiếp. Nền tảng hướng đến minh bạch thông tin và tăng khả năng kết nối giữa người cần hỗ trợ và cộng đồng.

🎯 Mục tiêu

Xây dựng web quản lý chiến dịch quyên góp theo mô hình MVC.
Cho phép đăng ký/đăng nhập, tạo chiến dịch, theo dõi và đóng góp.
Mở rộng được các chức năng quản trị, kiểm duyệt và lịch sử giao dịch.
🧩 Ý nghĩa

Hỗ trợ kêu gọi từ thiện minh bạch, dễ tiếp cận.
Khuyến khích cộng đồng cùng tham gia, theo dõi tiến độ rõ ràng.
Là nền tảng mở để phát triển thêm báo cáo, quản trị, thống kê.
🧪 Chức năng đã triển khai

Đăng ký, đăng nhập, đăng xuất (Cookie Authentication).
Danh sách chiến dịch, xem chi tiết.
Tạo chiến dịch mới (upload ảnh, mục tiêu, ngày kết thúc).
Ủng hộ chiến dịch và cập nhật tổng tiền hiện có.
Mô hình dữ liệu đầy đủ: Users, Campaigns, Donations, Comments, Reports, Follows, Notifications, AuditLogs, AdminActions.
🛠️ Công nghệ sử dụng

Backend: ASP.NET Core MVC (.NET 8)
ORM: Entity Framework Core 9
Database: SQL Server
UI: Bootstrap 5, jQuery
Auth/Session: Cookie Authentication + Session
🏗️ Kiến trúc hệ thống

Mô hình MVC: Controllers → Views → Models
EF Core quản lý dữ liệu và quan hệ
Session + Cookie Auth cho xác thực người dùng
🗂️ Cấu trúc dự án

QLTT/
├── Controllers/        # Luồng nghiệp vụ
├── Models/             # Entity + ViewModels
├── Data/               # AppDbContext & mapping
├── Views/              # Razor views
├── Migrations/         # EF Core migrations
├── wwwroot/            # CSS/JS/Images/Uploads
└── Program.cs          # App pipeline & config
🚧 Chức năng đang hoàn thiện

Bình luận, báo cáo, theo dõi chiến dịch (form đã có UI).
Trang quản trị: duyệt bài, xử lý báo cáo, quản lý user.
Hồ sơ cá nhân, lịch sử quyên góp, chiến dịch của tôi.
Trang Donate UI hiện còn trống.
🏁 Hướng phát triển tiếp theo

Mã hóa mật khẩu (BCrypt/ASP.NET Identity).
Hoàn thiện Comment/Report/Follow controllers.
Thêm phân quyền Admin + chức năng duyệt bài.
Thống kê chiến dịch và báo cáo hệ thống.
