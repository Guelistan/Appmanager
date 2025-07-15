using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using AppManager.Data; // Für AppUser

namespace AppManager.Pages.Admin
{
    public class UsersModel : PageModel
    {
        private readonly AppDbContext _context;

        public UsersModel(AppDbContext context)
        {
            _context = context;
        }

        public List<AppUser> Users { get; set; } = new();

        [BindProperty]
        public AppUser NewUser { get; set; }

        [BindProperty]
        public AppUser EditUser { get; set; } = new AppUser();

        public void OnGet()
        {
            Users = _context.Users.OrderByDescending(u => u.CreatedAt).ToList();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            NewUser.CreatedAt = DateTime.Now;
            _context.Users.Add(NewUser);
            _context.SaveChanges();
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(string id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            if (!ModelState.IsValid) return Page();

            var user = _context.Users.Find(EditUser.Id);
            if (user != null)
            {
                user.UserName = EditUser.UserName;
                user.Email = EditUser.Email;
                user.IsActive = EditUser.IsActive;
                _context.SaveChanges();
            }
            return RedirectToPage();
        }
    }
}
