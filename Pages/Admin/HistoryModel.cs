using System.Collections.Generic;
using AppManager.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppManager.Models;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore;
namespace AppManager.Pages.Admin
{


    public class HistoryModel : PageModel
    {
        private readonly AppDbContext _db;
        public List<LoggingOptions> Logs { get; set; }


        public HistoryModel(AppDbContext db) => _db = db;

        public void OnGet()
        {
            
        }
    }

}