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

var builder = WebApplication.CreateBuilder(args);

// 📧 Fake E-Mail-Sender für Entwicklung
builder.Services.AddTransient<IEmailSender, ConsoleEmailSender>();

// 📦 Datenbank mit SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔐 Identity-Konfiguration
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true;
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

var app = builder.Build();

// ⚠️ Fehlerbehandlung
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// 📡 Middleware-Pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

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

// 🚀 Anwendung starten
app.Run();
