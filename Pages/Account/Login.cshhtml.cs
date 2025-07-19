using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppManager.Data;
using AppManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using static AppManager.Data.AppDbContext;
using System;
using Microsoft.AspNetCore.Identity;

public class LoginModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<AppUser> _passwordHasher;

    public LoginModel(AppDbContext db, IPasswordHasher<AppUser> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == Input.Username);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Benutzername oder Passwort ist falsch.");
            return Page();
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Input.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Benutzername oder Passwort ist falsch.");
            return Page();
        }

        await _db.SaveChangesAsync();

        return RedirectToPage("/Index");
    }
}
