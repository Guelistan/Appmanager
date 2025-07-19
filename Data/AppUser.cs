using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AppManager.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace AppManager.Data
{
    public class AppUser : IdentityUser
    {
        public bool IsGlobalAdmin { get; set; }

        public bool IsActive { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string Abteilung { get; set; }
        public System.DateTime CreatedAt { get; set; } = System.DateTime.Now;
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedAt { get; set; } = System.DateTime.Now;
        // weitere Felder nach Bedarf
    }

    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        //public new DbSet<AppUser> Users { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ActivityLog> Logs { get; set; }

        public List<AppUser> GetUsersOrderedByCreationDate()
        {
            return Users.OrderByDescending(u => u.CreatedAt).ToList();
        }

        public class ActivityLog
        {
            public int Id { get; set; }
            public string UserId { get; set; }
        }
    }
}
