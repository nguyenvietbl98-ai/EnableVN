using Application;
using InfrastructureInMemory;
using InfrastructureSqlite.SeedData;
using Microsoft.Extensions.Options;
using Ports.Outbound.Services;
using Presentation.Options;
using Presentation.Services;
using InfrastructureSqlite;

var builder = WebApplication.CreateBuilder(args);

// MVC + Razor View.
builder.Services.AddControllersWithViews();

builder.Services.Configure<GeminiOptions>(builder.Configuration.GetSection(GeminiOptions.SectionName));
builder.Services.AddAntiforgery(options =>
{
    // Fetch / XMLHttpRequest thường gửi token qua header này (khớp với wwwroot/js/ai-recruitment.js).
    options.HeaderName = "X-XSRF-TOKEN";
});
builder.Services.AddHttpClient<GeminiClient>((sp, http) =>
{
    var opt = sp.GetRequiredService<IOptions<GeminiOptions>>().Value;
    http.Timeout = TimeSpan.FromSeconds(Math.Clamp(opt.RequestTimeoutSeconds, 30, 180));
});
builder.Services.AddScoped<AiRecruitmentService>();

// Session dùng để lưu UserId, Email, Role sau khi login/register.
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

// Đăng ký InMemory infrastructure cho services (password hasher, token service, etc.)
// mà chưa có bản SQLite.
builder.Services.AddEnableVNInMemoryInfrastructure();

// Ghi đè repositories bằng SQLite implementation.
builder.Services.AddEnableVNSqliteInfrastructure(builder.Configuration);

// Ghi đè ICurrentUserService mặc định của InMemory.
// Trong MVC, current user phải đọc từ Session thay vì Singleton RAM.
builder.Services.AddScoped<ICurrentUserService, SessionCurrentUserService>();

var app = builder.Build();

// Seed Admin + Catalog mặc định cho môi trường Development.
if (app.Environment.IsDevelopment())
{
    await SqliteAdminSeeder.SeedAsync(app.Services);
    await SqliteCatalogSeeder.SeedAsync(app.Services);
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
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

app.Run();