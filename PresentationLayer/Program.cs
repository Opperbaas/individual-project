using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using DataAccesLayer.Database;
using DataAccesLayer.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAccountRepository, SqlAccountRepository>();
builder.Services.AddScoped<IPlanningRepository, SqlPlanningRepository>();
builder.Services.AddScoped<ISportEventRepository, SqlSportEventRepository>();
builder.Services.AddScoped<ScheduleValidationService>();
builder.Services.AddScoped<PlanningService>();
builder.Services.AddScoped<SportEventService>();
builder.Services.AddScoped<IPasswordHasher<AccountUserRecord>, PasswordHasher<AccountUserRecord>>();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
