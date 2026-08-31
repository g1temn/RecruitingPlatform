using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/app-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting the application...");

    DotNetEnv.Env.Load();

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                           ?? builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<RecruitingPlatformDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddIdentity<User, UserRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<RecruitingPlatformDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(3);
        options.SlidingExpiration = true;
        options.LoginPath = "/Auth/LogIn";
        options.AccessDeniedPath = "/Home/Index";
    });

    builder.Services.AddCustomServices();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseExceptionHandler("/Home/Error");

    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthorization();
    app.MapStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    await AdminSeeder.SeedAsync(app.Services);

    await app.RunAsync();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed during the start");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}

public partial class Program { }
