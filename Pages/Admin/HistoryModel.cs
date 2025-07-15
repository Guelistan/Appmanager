using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using AppManager.Models;
using AppManager.Data;
using System.Linq;

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

        public void OnGet()
        {
            History = _context.Set<AppLaunchHistory>().ToList();
        }
    }
}