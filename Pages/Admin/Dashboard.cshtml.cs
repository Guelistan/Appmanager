using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using AppManager.Data;
using AppManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace AppManager.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public DashboardModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Application> Applications { get; set; } = new();

        public async Task OnGetAsync()
        {
            Applications = await _context.Applications
                .OrderByDescending(a => a.LastLaunchTime)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(int AppId, string action)
        {
            var app = await _context.Applications.FindAsync(AppId);
            if (app == null) return NotFound();

            switch (action)
            {
                case "start":
                    app.IsStarted = true;
                    app.LastLaunchReason = "Manuell gestartet";
                    app.LastLaunchTime = DateTime.Now;
                    break;
                case "stop":
                    app.IsStarted = false;
                    app.LastLaunchReason = "Manuell gestoppt";
                    break;
                case "restart":
                    app.IsStarted = true;
                    app.RestartRequired = false;
                    app.LastLaunchReason = "Manuell neu gestartet";
                    app.LastLaunchTime = DateTime.Now;
                    break;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
