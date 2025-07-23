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
using System;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// 📧 Fake E-Mail-Sender für Entwicklung
builder.Services.AddTransient<IEmailSender, ConsoleEmailSender>();

// 📦 Datenbank mit SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔐 Identity-Konfiguration
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedEmail = false;  // Username-Login ohne E-Mail-Bestätigung
        options.User.RequireUniqueEmail = false;       // Username als primärer Login

        // 🔓 Gelockerte Passwort-Richtlinien für einfache Registrierung
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 3;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 🍪 Authentifizierung via Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
});

// 📄 Razor Pages aktivieren
builder.Services.AddRazorPages();

// 📋 HTTP-Logging aktivieren
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
});

// Fehlende Service-Registrierung hinzufügen:
builder.Services.AddScoped<ProgramManagerService>();

var app = builder.Build();

// ⚠️ Fehlerbehandlung
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// 📡 Middleware-Pipeline

// 🧪 Initiales Datenbank-Seeding (Rollen, Admin, Anwendungen)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    context.Database.Migrate();

    // 🔧 Korrigiere fehlerhafte App-Pfade direkt in der Datenbank
    var existingApps = context.Applications.ToList();
    foreach (var appToFix in existingApps)
    {
        if (appToFix.ExecutablePath.StartsWith("/Desktop/") || !appToFix.ExecutablePath.Contains(@"\"))
        {
            string correctedPath = appToFix.ExecutablePath switch
            {
                "/Desktop/Browser" => @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                "/Desktop/Notepad" => @"C:\Windows\System32\notepad.exe",
                "/Desktop/Calculator" => @"C:\Windows\System32\calc.exe",
                "/Desktop/WetterApp" => @"C:\Windows\System32\mspaint.exe",
                _ when appToFix.Name.Contains("Browser") => @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                _ when appToFix.Name.Contains("Notepad") => @"C:\Windows\System32\notepad.exe",
                _ when appToFix.Name.Contains("Calculator") => @"C:\Windows\System32\calc.exe",
                _ when appToFix.Name.Contains("Paint") || appToFix.Name.Contains("Wetter") => @"C:\Windows\System32\mspaint.exe",
                _ when appToFix.Name.Contains("Manager") => @"C:\Windows\System32\taskmgr.exe",
                _ => appToFix.ExecutablePath
            };

            if (correctedPath != appToFix.ExecutablePath)
            {
                Console.WriteLine($"✅ Korrigiere: '{appToFix.ExecutablePath}' → '{correctedPath}'");
                appToFix.ExecutablePath = correctedPath;
                if (string.IsNullOrEmpty(appToFix.WorkingDirectory))
                {
                    appToFix.WorkingDirectory = @"C:\Windows\System32";
                }
            }
        }
    }
    context.SaveChanges();

    // Anwendungen seeden
    if (!context.Applications.Any())
    {
        var apps = new List<Application>
        {
            new() { Name = "Rechner", Description = "Windows Rechner", ExecutablePath = @"C:\Windows\System32\calc.exe", WorkingDirectory = @"C:\Windows\System32" },
            new() { Name = "Notepad", Description = "Windows Editor", ExecutablePath = @"C:\Windows\System32\notepad.exe", WorkingDirectory = @"C:\Windows\System32" },
            new() { Name = "Paint", Description = "Windows Paint", ExecutablePath = @"C:\Windows\System32\mspaint.exe", WorkingDirectory = @"C:\Windows\System32" },
            new() { Name = "Task Manager", Description = "Windows Task Manager", ExecutablePath = @"C:\Windows\System32\taskmgr.exe", WorkingDirectory = @"C:\Windows\System32", RequiresAdmin = true },
            new() { Name = "Command Prompt", Description = "Eingabeaufforderung", ExecutablePath = @"C:\Windows\System32\cmd.exe", WorkingDirectory = @"C:\Windows\System32" }
        };

        context.Applications.AddRange(apps);
        context.SaveChanges();
    }



    var allApps = context.Applications.ToList();
    Console.WriteLine("Apps in DB:");
    foreach (var a in allApps)
    {
        Console.WriteLine($"- {a.Name}");
    }

    // Rollen anlegen
    string[] roles = { "Admin", "SuperAdmin" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Hauptadmin anlegen
    var username = "admin";
    var email = "admin@appmanager.local";
    var password = "Admin123!";

    var admin = await userManager.FindByNameAsync(username);
    if (admin == null)
    {
        admin = new AppUser
        {
            UserName = username,
            Email = email,
            EmailConfirmed = true,
            Vorname = "Administrator",
            Nachname = "System",
            IsActive = true,
            IsGlobalAdmin = true
        };
        await userManager.CreateAsync(admin, password);
    }

    if (!await userManager.IsInRoleAsync(admin, "SuperAdmin"))
    {
        await userManager.AddToRoleAsync(admin, "SuperAdmin");
    }

    // 🧪 Test-Daten nur für Development
    if (app.Environment.IsDevelopment())
    {
        await AppManager.TestDataSeeder.SeedTestDataAsync(services);
    }

    // 🚀 Produktions-Basisdaten für alle Umgebungen
    await AppManager.ProductionSeeder.SeedEssentialDataAsync(services);

    // 🔍 Debug: Benutzer-Datenbank überprüfen
    Console.WriteLine();
    using (var debugScope = services.CreateScope())
    {
        var debugContext = debugScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var debugUserManager = debugScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        await AppManager.DebugUserCheck.CheckUsersInDatabase(debugContext, debugUserManager);
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();
// 🚀 Anwendung starten
app.Run();
