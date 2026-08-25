using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecruitingPlatform.Data;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Services.Auth;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.Services.Vacancies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<RecruitingPlatformDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
});

builder.Services.AddScoped<ILogInService, LogInService>();
builder.Services.AddScoped<ISignJobSeekerUpService, SignJobSeekerUpService>();
builder.Services.AddScoped<ISignEmployerUpService, SignEmployerUpService>();
builder.Services.AddScoped<ICheckEmailExsistsService, CheckEmailExistsService>();
builder.Services.AddScoped<IGetVacanciesWithFiltersService, GetVacanciesWithFiltersService>();
builder.Services.AddScoped<IGetVacancyByIdService, GetVacancyByIdService>();
builder.Services.AddScoped<IGetResumesWithFiltersService, GetResumesWithFiltersService>();
builder.Services.AddScoped<IGetResumeByIdService, GetResumeByIdService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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


await app.RunAsync();
