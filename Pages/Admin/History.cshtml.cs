using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AppManager.Data;
using AppManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppManager.Pages.Admin
{
    public class HistoryModel : PageModel
    {
        private readonly AppDbContext _context;

        public HistoryModel(AppDbContext context)
        {
            _context = context;
        }

        public List<AppLaunchHistory> History { get; set; }

        public async Task OnGetAsync()
        {
            History = await _context.AppLaunchHistories
                .AsNoTracking()
                .ToListAsync();
        }


        public class ActivityLog
        {
            public int Id { get; set; }

            public string UserId { get; set; }

            [ForeignKey("UserId")]
            public AppUser User { get; set; }
        }

    }
}
