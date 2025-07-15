using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppManager.Models;
using Microsoft.AspNetCore.Identity;

namespace AppManager.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<AppLaunchHistory> AppLaunchHistories { get; set; }
        // KEIN weiteres DbSet<User>!
    }
}

namespace AppManager.Models
{
    public class AppUser : IdentityUser
    {
        public bool IsActive { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }
}
