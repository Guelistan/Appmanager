using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppManager.Data;

namespace AppManager.Pages.Admin
{
    public class UsersModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersModel(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = new AppUser
            {
                UserName = NewUser.Email,
                Email = NewUser.Email,
                Vorname = NewUser.Vorname,
                Abteilung = NewUser.Abteilung,
                IsActive = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var tempPassword = "Default123!";

            var result = await _userManager.CreateAsync(user, tempPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            await _userManager.AddToRoleAsync(user, "Admin");

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.FindByIdAsync(EditUser.Id);
            if (user != null)
            {
                user.UserName = EditUser.Email;
                user.Email = EditUser.Email;
                user.IsActive = EditUser.IsActive;
                user.Vorname = EditUser.Vorname;
                user.Abteilung = EditUser.Abteilung;
                user.UpdatedAt = DateTime.Now;

                await _userManager.UpdateAsync(user);
            }

            return RedirectToPage();
        }
    }
}
