using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppManager.Data
{
    public class AppUser : IdentityUser
    {
        public bool IsActive { get; set; }
        public string Vorname { get; set; }
        public string Abteilung { get; set; }
        public System.DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // weitere Felder nach Bedarf
    }

    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public new DbSet<AppUser> Users { get; set; }

        public List<AppUser> GetUsersOrderedByCreationDate()
        {
            return Users.OrderByDescending(u => u.CreatedAt).ToList();
        }
    }
}
