using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AppManager.Data;
using AppManager.Models;
using AppManager.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace AppManager.Pages.Admin
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ProgramManagerService _programManager;

        public DashboardModel(AppDbContext context, UserManager<AppUser> userManager, ProgramManagerService programManager)
        {
            _context = context;
            _userManager = userManager;
            _programManager = programManager;
        }

        public List<Application> Applications { get; set; } = new();
        public List<AppLaunchHistory> LaunchHistory { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Apps laden und Status aktualisieren
            Applications = await _context.Applications.ToListAsync();

            // Status aller Apps überprüfen
            foreach (var app in Applications)
            {
                app.IsStarted = _programManager.IsProgramRunning(app);
            }

            await _context.SaveChangesAsync();

            // History laden
            LaunchHistory = await _context.AppLaunchHistories
                .Include(h => h.User)
                .Include(h => h.Application)
                .OrderByDescending(h => h.LaunchTime)
                .Take(10)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostStartAsync(Guid appId, string customReason) // ← Kein ? mehr
        {
            var app = await _context.Applications.FindAsync(appId);
            if (app == null) return NotFound();

            // 🚀 ECHTES Programm starten
            bool success = await _programManager.StartProgramAsync(app);

            var history = new AppLaunchHistory
            {
                ApplicationId = appId,
                UserId = _userManager.GetUserId(User) ?? string.Empty,
                LaunchTime = DateTime.Now,
                Action = "Start",
                Reason = success
                    ? (!string.IsNullOrWhiteSpace(customReason) ? customReason : "Manuell gestartet")
                    : "Start fehlgeschlagen"
            };

            _context.AppLaunchHistories.Add(history);
            await _context.SaveChangesAsync();

            if (!success)
            {
                TempData["Error"] = $"Programm '{app.Name}' konnte nicht gestartet werden.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostStopAsync(Guid appId, string customReason) // ← Kein ? mehr
        {
            var app = await _context.Applications.FindAsync(appId);
            if (app == null) return NotFound();

            // 🛑 ECHTES Programm stoppen
            bool success = await _programManager.StopProgramAsync(app);

            var history = new AppLaunchHistory
            {
                ApplicationId = appId,
                UserId = _userManager.GetUserId(User) ?? string.Empty,
                LaunchTime = DateTime.Now,
                Action = "Stop",
                Reason = success
                    ? (!string.IsNullOrWhiteSpace(customReason) ? customReason : "Manuell gestoppt")
                    : "Stop fehlgeschlagen"
            };

            _context.AppLaunchHistories.Add(history);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRestartAsync(Guid appId, string customReason) // ← Kein ? mehr
        {
            var app = await _context.Applications.FindAsync(appId);
            if (app == null) return NotFound();

            // 🔄 ECHTES Programm neustarten
            bool success = await _programManager.RestartProgramAsync(app);

            var history = new AppLaunchHistory
            {
                ApplicationId = appId,
                UserId = _userManager.GetUserId(User) ?? string.Empty,
                LaunchTime = DateTime.Now,
                Action = "Restart",
                Reason = success
                    ? (!string.IsNullOrWhiteSpace(customReason) ? customReason : "Manuell neu gestartet")
                    : "Restart fehlgeschlagen"
            };

            _context.AppLaunchHistories.Add(history);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
