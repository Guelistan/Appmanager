using Microsoft.EntityFrameworkCore;
using AppManager.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI;
using System.Linq;
using AppManager.Models;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);


// Datenbank konfigurieren
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// Keine Identity hinzufügen, da eigene Login-Logik verwendet wird
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



using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Nur Dummy-Daten hinzufügen, wenn noch keine vorhanden sind
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
        context.SaveChanges();
    }
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
