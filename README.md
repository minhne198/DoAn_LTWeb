# QLTT – N?n t?ng kêu g?i quyên góp t? thi?n

D? án web ASP.NET Core MVC h? tr? t?o chi?n d?ch kêu g?i, dang nh?p/dang ký và qu?n lý quyên góp. Ðây là phiên b?n h?c t?p v?i m?t s? tính nang dã hoàn thi?n và m?t s? ph?n dang ti?p t?c phát tri?n.

## Tính nang hi?n có
- Ðang ký, dang nh?p, dang xu?t (Cookie Authentication).
- Danh sách chi?n d?ch và xem chi ti?t chi?n d?ch.
- T?o chi?n d?ch kêu g?i (có upload ?nh, m?c tiêu, ngày k?t thúc).
- T?o quyên góp và c?p nh?t s? ti?n hi?n có c?a chi?n d?ch.
- Mô hình d? li?u d?y d?: Users, Campaigns, Donations, Comments, Reports, Follows, Notifications, AuditLogs, AdminActions.

## Tính nang dang hoàn thi?n
- Bình lu?n, báo cáo, theo dõi chi?n d?ch (form dã có ? trang chi ti?t).
- Trang qu?n tr? (Dashboard, duy?t bài, x? lý báo cáo, qu?n lý ngu?i dùng).
- Trang h? so cá nhân, l?ch s? quyên góp, chi?n d?ch c?a tôi.
- Giao di?n trang ?ng h? (Donate) hi?n còn tr?ng.

## Công ngh? s? d?ng
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core 9 + SQL Server
- Cookie Authentication + Session
- Bootstrap 5, jQuery

## C?u trúc thu m?c
| Thu m?c | Mô t? |
|---|---|
| `Controllers/` | X? lý các lu?ng nghi?p v? (Account, Campaign, Donation, …) |
| `Models/` | Entity models và ViewModels |
| `Data/` | `AppDbContext` và c?u hình quan h? d? li?u |
| `Migrations/` | EF Core migrations |
| `Views/` | Giao di?n Razor |
| `wwwroot/` | Tài nguyên tinh (css/js/lib/uploads) |

## Yêu c?u h? th?ng
- .NET SDK 8.0
- SQL Server (LocalDB ho?c SQL Server d?y d?)
- (Tùy ch?n) công c? `dotnet-ef` d? ch?y migration

## C?u hình
1. C?p nh?t chu?i k?t n?i trong `QLTT/appsettings.json`:
   - M?c d?nh: `Server=DESKTOP-Q5CJ9BF;Database=QLTT;Trusted_Connection=True;TrustServerCertificate=True;`
2. Thu m?c upload ?nh: `QLTT/wwwroot/uploads` (dã du?c t?o s?n).

## Cài d?t và ch?y d? án
T? thu m?c g?c d? án:

```bash
cd QLTT

dotnet restore

dotnet ef database update

dotnet run
```

Truy c?p ?ng d?ng t?i:
- http://localhost:5000
- https://localhost:7101

## Lu?ng s? d?ng co b?n
1. Ðang ký tài kho?n m?i.
2. Ðang nh?p d? s? d?ng các ch?c nang yêu c?u xác th?c.
3. T?o chi?n d?ch quyên góp.
4. Xem danh sách và chi ti?t chi?n d?ch.
5. ?ng h? (ghi nh?n vào CSDL, c?p nh?t t?ng ti?n chi?n d?ch).

## Ghi chú quan tr?ng
- M?t kh?u hi?n dang luu d?ng plain text (chua mã hóa). C?n thay b?ng hashing (BCrypt/ASP.NET Identity) khi dùng th?c t?.
- T?o chi?n d?ch dang gán `UserId = 1` (chua l?y t? session/claims).
- Trang Donate và m?t s? trang Admin/Profile ch? là khung r?ng (placeholder).
- Form bình lu?n/báo cáo/theo dõi dã có ? giao di?n nhung controller x? lý chua du?c tri?n khai.

## G?i ý phát tri?n ti?p
- Áp d?ng `[Authorize]` cho các action c?n dang nh?p.
- Hoàn thi?n Comment/Report/Follow controllers + validation.
- Thêm phân quy?n admin, duy?t bài, khóa tài kho?n.
- Thêm seed data và d? li?u demo.
- B? sung UI cho Donation, Profile, Admin.

---
N?u anh mu?n em ti?p t?c hoàn thi?n ph?n nào (ví d?: Comment/Report/Follow, trang Donate, hay phân quy?n Admin), c? nói là em làm ti?p ngay.
