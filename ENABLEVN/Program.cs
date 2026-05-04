using Application;
using InfrastructureInMemory;
using Ports.Outbound.Services;
using Presentation.Services;
using Presentation.SeedData;


var builder = WebApplication.CreateBuilder(args);

// MVC + Razor View.
builder.Services.AddControllersWithViews();

// Session dùng để lưu UserId, Email, Role sau khi login/register.
// Đây là bản đơn giản để test MVC với InMemory.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cho phép service đọc HttpContext hiện tại.
builder.Services.AddHttpContextAccessor();

// Đăng ký Application UseCases.
builder.Services.AddEnableVNApplication();

// Đăng ký repository/service InMemory.
builder.Services.AddEnableVNInMemoryInfrastructure();

// Ghi đè ICurrentUserService mặc định của InMemory.
// Trong MVC, current user nên đọc từ Session thay vì Singleton RAM.
builder.Services.AddScoped<ICurrentUserService, SessionCurrentUserService>();

var app = builder.Build();
// Seed Admin mặc định cho môi trường Development/InMemory.
// Email: admin@enablevn.local
// Password: Admin@123
if (app.Environment.IsDevelopment())
{
    await InMemoryAdminSeeder.SeedAsync(app.Services);
    await InMemoryCatalogSeeder.SeedAsync(app.Services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();