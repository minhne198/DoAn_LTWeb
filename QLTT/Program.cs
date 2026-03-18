using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using QLTT.Data;

/// <summary>
/// Program.cs - Điểm khởi động (Startup) của ứng dụng ASP.NET Core.
/// 
/// Mục đích:
/// - Cấu hình Dependency Injection (DI) container
/// - Cấu hình middleware pipeline
/// - Cấu hình services (database, authentication, session, etc.)
/// - Định tuyến (routing) mặc định
/// 
/// Tổng quan quy trình:
/// 1. Tạo WebApplicationBuilder
/// 2. Thêm các services vào DI container (AddServices)
/// 3. Build WebApplication
/// 4. Cấu hình middleware pipeline (Use/Map middleware)
/// 5. Chạy ứng dụng (app.Run())
/// 
/// LƯU Ý:
/// - Order của middleware rất quan trọng (exception handler → routing → auth → session)
/// - Services được đã "Add" có thể được inject vào controllers/services
/// - Middleware được "Use" sẽ xử lý từng HTTP request
/// </summary>

var builder = WebApplication.CreateBuilder(args);

// ==================== SERVICES CONFIGURATION ====================
// (Đăng ký các services vào Dependency Injection container)

/// <summary>
/// AddControllersWithViews() - Đăng ký MVC (Model-View-Controller)
/// 
/// Bao gồm:
/// - Controllers: Xử lý logic HTTP requests
/// - Views: Razor templates (.cshtml) để render HTML
/// - Models: Data models
/// 
/// Mà KHÔNG bao gồm:
/// - API endpoints (dùng AddControllers() hoặc MapControllers() cho API)
/// 
/// Service instance: Scoped (tạo mới cho mỗi HTTP request)
/// </summary>
builder.Services.AddControllersWithViews();

// Thêm Logging
// Logger<T> sẽ được auto-inject vào constructor nếu khai báo
builder.Services.AddLogging(config =>
{
    // Có thể cấu hình log level, output, etc.
    config.AddConsole();
    config.AddDebug();
});

/// <summary>
/// AddDbContext<AppDbContext>() - Đăng ký Entity Framework Core
/// 
/// Tác dụng:
/// - DbContext được inject vào controllers/services
/// - Quản lý database connection
/// - Tracking entity changes
/// - LINQ queries trên database
/// 
/// Options.UseSqlServer():
/// - Sử dụng SQL Server làm database provider
/// - Connection string lấy từ configuration (appsettings.json)
/// 
/// Service instance: Scoped (tạo mới cho mỗi HTTP request)
/// </summary>
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/// <summary>
/// AddAuthentication() - Đăng ký Authentication service
/// 
/// Mục đích:
/// - Xác thực danh tính người dùng (Who are you?)
/// - Tạo và quản lý authentication cookies/tokens
/// - Kiểm tra [Authorize] attribute trên controllers/actions
/// 
/// CookieAuthenticationDefaults.AuthenticationScheme:
/// - Sử dụng Cookie làm phương pháp authentication
/// - (Có thể dùng JWT, OAuth, OpenID, v.v.)
/// 
/// Cấu hình AddCookie():
/// - LoginPath: Chuyển hướng đến đây nếu chưa đăng nhập + [Authorize]
/// - LogoutPath: Chuyển hướng đến đây khi người dùng click Logout
/// - ExpireTimeSpan: Cookie hết hạn sau 30 phút không hoạt động
/// - SlidingExpiration: Tự động gia hạn cookie nếu user vẫn đang hoạt động
/// 
/// Ví dụ flow:
/// 1. User đăng nhập → Tạo cookie
/// 2. User truy cập [Authorize] page → Cookie được kiểm tra
/// 3. Nếu cookie còn hợp lệ → Cho phép truy cập
/// 4. Nếu cookie hết hạn → Chuyển hướng đến LoginPath
/// </summary>
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Trang đăng nhập - chuyển hướng khi cần authenticate
        options.LoginPath = "/Account/Login";
        
        // Trang đăng xuất - chuyển hướng sau khi logout
        options.LogoutPath = "/Account/Logout";
        
        // Cookie tồn tại 30 phút không hoạt động
        // Hết hạn nếu user không tương tác trong 30 phút
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        
        // Tự động gia hạn cookie nếu còn trong vòng 1 nửa thời gian (15 phút)
        // Tức là: nếu user hoạt động sau 15 phút, cookie được reset 30 phút mới
        // Nếu user không hoạt động 15 phút, phải đăng nhập lại
        options.SlidingExpiration = true;
    });

/// <summary>
/// AddSession() - Đăng ký Session service
/// 
/// Mục đích:
/// - Lưu trữ dữ liệu trên server cho từng user
/// - Dữ liệu có thể truy cập qua HttpContext.Session
/// 
/// Ví dụ:
/// HttpContext.Session.SetString("UserName", "John");
/// var name = HttpContext.Session.GetString("UserName");
/// 
/// Cấu hình:
/// - IdleTimeout: Session hết hạn nếu không hoạt động 30 phút
/// - Cookie.HttpOnly: JS không thể access cookie (bảo mật chống XSS)
/// - Cookie.IsEssential: Bắt buộc lưu cookie ngay cả khi user từ chối
/// 
/// Lưu ý: Session khác với Cookie Authentication
/// - Cookie Auth: Xác thực danh tính (ai đang đăng nhập)
/// - Session: Lưu dữ liệu tạm thời (username hiển thị, giỏ hàng, etc.)
/// 
/// LƯU Ý: Session store mặc định là in-memory (mất khi restart ứng dụng)
/// Với production, nên dùng SQL Server session store
/// </summary>
builder.Services.AddSession(options =>
{
    // Session tồn tại 30 phút nếu không hoạt động
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    
    // HttpOnly = true: Ngăn JavaScript truy cập cookie
    // Mục đích: Chống XSS (Cross-Site Scripting) attack
    // Hacker không thể steal session cookie qua JS
    options.Cookie.HttpOnly = true;
    
    // IsEssential = true: Bắt buộc lưu cookie
    // Mục đích: GDPR compliance - luôn lưu session ngay cả khi user từ chối cookie
    options.Cookie.IsEssential = true;
});

// ==================== BUILD APP ====================
// Tạo WebApplication instance từ builder
var app = builder.Build();

// ==================== MIDDLEWARE PIPELINE CONFIGURATION ====================
// (Cấu hình thứ tự xử lý HTTP request)

/// <summary>
/// Middleware pipeline - Thứ tự xử lý HTTP request RẤT QUAN TRỌNG!
/// 
/// Quy trình (theo thứ tự):
/// 1. Exception Handler - Bắt lỗi và hiển thị error page
/// 2. HTTPS Redirect - Chuyển HTTP → HTTPS
/// 3. Static Files - Phục vụ CSS, JS, ảnh (wwwroot/)
/// 4. Routing - Xác định Controller/Action nào xử lý request
/// 5. Authentication - Xác thực người dùng (read cookie)
/// 6. Authorization - Kiểm tra quyền truy cập ([Authorize])
/// 7. Session - Khích hoạt session support
/// 8. Endpoint - Thực thi method của controller
/// 
/// LƯU Ý: Middleware KHÔNG được return sau khi xử lý
/// Nó gọi next() để chuyển request đến middleware tiếp theo (pipeline)
/// </summary>

// Exception Handler - Bắt tất cả unhandled exceptions
// Chỉ được kích hoạt trong Production (không phải Development)
// Development sẽ hiển thị detailed error page
if (!app.Environment.IsDevelopment())
{
    // Chuyển hướng đến /Home/Error khi có unhandled exception
    app.UseExceptionHandler("/Home/Error");
    
    // HSTS (HTTP Strict Transport Security)
    // Báo cho browser: Chỉ dùng HTTPS, không bao giờ HTTP
    // Bảo vệ chống downgrade attack (buộc HTTPS)
    // MaxAge: 1 năm (31536000 giây)
    app.UseHsts();
}

// HTTPS Redirect
// Tự động chuyển hướng HTTP requests → HTTPS
// Ví dụ: http://example.com → https://example.com
app.UseHttpsRedirection();

// Static Files Middleware
// Phục vụ tập tin tĩnh (CSS, JS, ảnh) từ wwwroot/ folder
// Không cần đi qua controller, trả về file trực tiếp
app.UseStaticFiles();

// Routing Middleware
// Phân tích URL và xác định Controller/Action/Parameter nào sẽ xử lý
// Phải được gọi TRƯỚC UseAuthentication() và UseAuthorization()
app.UseRouting();

// Authentication Middleware
// Đọc authentication cookies và xác thực người dùng
// Thiết lập HttpContext.User từ cookie
// PHẢI được gọi TRƯỚC UseAuthorization()
app.UseAuthentication();

// Authorization Middleware
// Kiểm tra [Authorize] attribute và [Authorize("Policy")]
// Nếu user không được phép → Return 403 Forbidden
// PHẢI được gọi AFTER UseAuthentication()
app.UseAuthorization();

// Session Middleware
// Kích hoạt Session support
// Cho phép HttpContext.Session được sử dụng
app.UseSession();

// ==================== ENDPOINT MAPPING ====================
// Định tuyến (routing) mặc định
/// <summary>
/// MapControllerRoute() - Định tuyến cho controllers
/// 
/// Pattern: "{controller=Campaign}/{action=Index}/{id?}"
/// Ý nghĩa:
/// - {controller=Campaign}: Tên controller (mặc định Campaign)
/// - {action=Index}: Tên action method (mặc định Index)
/// - {id?}: Parameter ID (tùy chọn, ? = nullable)
/// 
/// Ví dụ URL:
/// / → CampaignController.Index()
/// /Campaign → CampaignController.Index()
/// /Campaign/Details/5 → CampaignController.Details(5)
/// /Account/Login → AccountController.Login()
/// /Donation/Donate/10 → DonationController.Donate(10)
/// </summary>
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Campaign}/{action=Index}/{id?}");

// ==================== RUN APP ====================
// Bắt đầu lắng nghe HTTP requests và chạy ứng dụng
app.Run();
