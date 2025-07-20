using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AppManager.Data;
using AppManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppManager.Pages.Admin
{
    public class HistoryModel : PageModel
    {
        private readonly AppDbContext _context;

        public HistoryModel(AppDbContext context)
        {
            _context = context;
        }

        public List<AppLaunchHistory> History { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Lade und sortiere die Start-Historie
            History = await _context.AppLaunchHistories
                .OrderByDescending(h => h.LaunchTime)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string action, Guid AppId)
        {
            var app = await _context.Applications.FindAsync(AppId);
            if (app == null)
            {
                ModelState.AddModelError("", "Anwendung nicht gefunden.");
                return Page();
            }

            // Anwendung je nach Aktion manipulieren
            switch (action?.ToLower())
            {
                case "start":
                    app.IsStarted = true;
                    break;
                case "stop":
                    app.IsStarted = false;
                    break;
                case "restart":
                    app.IsStarted = true; // Option: vorübergehend stoppen
                    break;
                default:
                    ModelState.AddModelError("", "Ungültige Aktion.");
                    return Page();
            }

            // History-Eintrag erzeugen
            // Assuming you have a way to get the user's integer ID, replace this with the correct logic.
            int? userId = null;
            if (User?.Identity?.Name != null)
            {
                // Example: Try to parse the user name as an int, or fetch from your user store
                if (int.TryParse(User.Identity.Name, out int parsedId))
                {
                    userId = parsedId;
                }
                // Otherwise, set to null or a default value
            }
            _ = _context.AppLaunchHistories.Add(new AppLaunchHistory
            {
                ApplicationId = app.Id,
                UserId = userId,
                LaunchTime = DateTime.Now,
                Reason = action
            });

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
