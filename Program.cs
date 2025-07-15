using Microsoft.EntityFrameworkCore;
using AppManager.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Linq;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDefaultIdentity<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();

// Datenbank konfigurieren
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// 🔧 Datenbank seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Applications.Any())
    {
        context.Applications.AddRange(
            new AppManager.Models.Application
            {
                Name = "App 1",
                IsStarted = false,
                RestartRequired = false
            },
            new AppManager.Models.Application
            {
                Name = "App 2",
                IsStarted = true,
                RestartRequired = true
            }
        );

        context.SaveChanges(); // ❗ wichtig, damit Daten gespeichert werden
    }
}

// 🌐 Jetzt wird die App IMMER gestartet – egal ob Daten existieren oder nicht
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
