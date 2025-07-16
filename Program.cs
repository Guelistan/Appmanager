using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using AppManager.Data;
using AppManager.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using AppManager.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IEmailSender, ConsoleEmailSender>();

// 📦 Datenbankkonfiguration mit SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔐 Identity mit deinem benutzerdefinierten AppUser
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 🍪 Cookie-basiertes Authentifizierungssystem
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });

// 📄 Razor Pages hinzufügen
builder.Services.AddRazorPages();

// 📋 HttpLogging aktivieren
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
});

var app = builder.Build();

// 🧪 Initiales Datenbank-Seeding (Benutzer, Rollen, Apps)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Anwendungseinträge
    if (!context.Applications.Any())
    {
        context.Applications.AddRange(
            new Application { Name = "App 1", IsStarted = false, RestartRequired = false },
            new Application { Name = "App 2", IsStarted = true, RestartRequired = true }
        );
        context.SaveChanges();
    }

    // Rollen erstellen
    string[] roles = { "Admin", "SuperAdmin" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Hauptadmin erstellen
    var email = "hauptadmin@app.com";
    var password = "Hauptadmin123!";

    var admin = await userManager.FindByEmailAsync(email);
    if (admin == null)
    {
        admin = new AppUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            Vorname = "Hauptadmin",
            IsActive = true
        };
        await userManager.CreateAsync(admin, password);
    }

    if (!await userManager.IsInRoleAsync(admin, "SuperAdmin"))
    {
        await userManager.AddToRoleAsync(admin, "SuperAdmin");
    }
}

// ⚠️ Ausnahmebehandlung im Produktivmodus
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();

// 🔁 Konventionelles Routing für MVC-Controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 📄 Razor Pages-Routen aktivieren
app.MapRazorPages();

// 🚀 Anwendung starten
app.Run();
