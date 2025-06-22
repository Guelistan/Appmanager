using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using AppManager.Data;
using AppManager.Models;
using System.Collections.Generic;
using System.Linq;

namespace AppManager.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public DashboardModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Application> Applications { get; set; } = new();

        public void OnGet()
        {
            Applications = _context.Applications.ToList();
        }

        public IActionResult OnPost(int AppId, string action)
        {
            var app = _context.Applications.FirstOrDefault(a => a.Id == AppId);
            if (app == null) return RedirectToPage();

            switch (action)
            {
                case "start":
                    app.IsStarted = true;
                    app.RestartRequired = false;
                    break;
                case "stop":
                    app.IsStarted = false;
                    break;
                case "restart":
                    app.IsStarted = true;
                    app.RestartRequired = false;
                    break;
            }

            _context.SaveChanges();
            return RedirectToPage();
        }
    }
}
