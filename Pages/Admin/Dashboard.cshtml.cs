using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using AppManager.Data;
using AppManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AppManager.Pages.Admin
{
    [Authorize] // Authentifizierung erforderlich
    public class DashboardModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public DashboardModel(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Application> Applications { get; set; } = new();
        // ✅ Historie für Anzeige im View
        public List<AppLaunchHistory> LaunchHistory { get; set; } = new();

        public async Task OnGetAsync()
        {
            Applications = await _context.Applications
                .OrderByDescending(a => a.LastLaunchTime)
                .ToListAsync();

            LaunchHistory = await _context.AppLaunchHistories
                .Include(h => h.Application)
                .OrderByDescending(h => h.LaunchTime)
                .Take(20)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(Guid AppId, string action)
        {
            var app = await _context.Applications.FindAsync(AppId);
            if (app == null) return NotFound();

            var now = DateTime.Now;
            var reason = "";

            switch (action)
            {
                case "start":
                    app.IsStarted = true;
                    reason = "Manuell gestartet";
                    app.LastLaunchTime = now;
                    break;
                case "stop":
                    app.IsStarted = false;
                    reason = "Manuell gestoppt";
                    break;
                case "restart":
                    app.IsStarted = true;
                    app.RestartRequired = false;
                    reason = "Manuell neu gestartet";
                    app.LastLaunchTime = now;
                    break;
            }

            app.LastLaunchReason = reason;

            _context.AppLaunchHistories.Add(new AppLaunchHistory
            {
                ApplicationId = AppId,
                UserId = User.Identity?.IsAuthenticated == true ? _userManager.GetUserId(User) : null,
                LaunchTime = now,
                Reason = reason
            });

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid appId)
        {
            var app = await _context.Applications.FindAsync(appId);
            if (app == null) return NotFound();

            _context.Applications.Remove(app);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
