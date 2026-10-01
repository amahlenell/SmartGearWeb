using SmartGearWeb.Data;
using SmartGearWeb.Middleware;
using SmartGearWeb.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SmartGearWeb.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ---- Register services (Dependency Injection) ----
builder.Services.AddControllersWithViews();

// Custom service registered via DI (see Services/IStockNotificationService.cs)

builder.Services.AddScoped<IStockNotificationService, StockNotificationService>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); 

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); 

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => { options.Password.RequiredLength = 6; options.Password.RequireNonAlphanumeric = false; }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<IImageUploadService, CloudinaryImageService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.AccessDeniedPath = "/account/access-denied";
});

builder.Services.AddSignalR(); 

var app = builder.Build();

SmartGearWeb.Data.SeedData.EnsurePopulated(app);

// ---- Render deployment: listen on the port Render assigns via $PORT ----
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

// ---- Built-in middleware ----
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ---- Custom middleware: logs the path of every incoming request ----
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<SmartGearWeb.Hubs.ProductHub>("/hubs/products"); 

app.Run();
