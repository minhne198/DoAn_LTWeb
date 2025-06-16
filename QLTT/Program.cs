using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using QLTT.Data;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ MVC (Controller + View)
builder.Services.AddControllersWithViews();

// Thêm AppDbContext (EF Core)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm dịch vụ Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

// Thêm dịch vụ Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session tồn tại trong 30 phút
    options.Cookie.HttpOnly = true; // Bảo mật
    options.Cookie.IsEssential = true; // Bắt buộc dùng cookie cho session
});

var app = builder.Build();

// Cấu hình pipeline xử lý HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();         // Chuyển HTTP -> HTTPS
app.UseStaticFiles();             // Cho phép dùng CSS, JS, ảnh tĩnh

app.UseRouting();                 // Kích hoạt routing

app.UseAuthentication();         // Kích hoạt authentication
app.UseAuthorization();          // Authorization

app.UseSession();                // Session support

// Định tuyến mặc định 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Campaign}/{action=Index}/{id?}");

app.Run();
