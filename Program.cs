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

// 📦 Datenbank mit SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔐 Identity-Konfiguration
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedEmail = false;  // Username-Login ohne E-Mail-Bestätigung
        options.User.RequireUniqueEmail = false;       // Username als primärer Login
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

    // Anwendungen seeden
    if (!context.Applications.Any())


    {
        context.Applications.AddRange(
            new Application { Name = "App Manager", IsStarted = true, RestartRequired = false },
            new Application { Name = "Test App", IsStarted = false, RestartRequired = true },
            new Application { Name = "Demo App", IsStarted = true, RestartRequired = false },
            new Application { Name = "Beispiel App", IsStarted = false, RestartRequired = true },
            new Application { Name = "App 1", IsStarted = false, RestartRequired = false },
            new Application { Name = "App 2", IsStarted = true, RestartRequired = true },
            new Application { Name = "/Desktop/Notepad", IsStarted = false, RestartRequired = false },
            new Application { Name = "/Desktop/Calculator", IsStarted = true, RestartRequired = false },
            new Application { Name = "/Desktop/Browser", IsStarted = false, RestartRequired = true },
            new Application { Name = "/Desktop/WetterApp", IsStarted = false, RestartRequired = true }
        );


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
}

var apps = new List<Application>
{
    new() { Name = "Rechner", Description = "Windows Rechner", ExecutablePath = "calc.exe" },
    new() { Name = "Notepad", Description = "Windows Editor", ExecutablePath = "notepad.exe" },
    new() { Name = "Paint", Description = "Windows Paint", ExecutablePath = "mspaint.exe" },
    new() { Name = "Task Manager", Description = "Windows Task Manager", ExecutablePath = "taskmgr.exe", RequiresAdmin = true },
    new() { Name = "Command Prompt", Description = "Eingabeaufforderung", ExecutablePath = "cmd.exe" }
};

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
