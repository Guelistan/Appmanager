using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppManager.Models;

namespace AppManager.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public DbSet<Application> Applications { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
