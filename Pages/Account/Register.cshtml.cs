/* using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using AppManager.Data;

namespace AppManager.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;

        public RegisterModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new();

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            // Use this if you want the one from Data
            AppManager.Data.AppUser user = new AppManager.Data.AppUser();

            user.UserName = Input.Name;
            user.Vorname = Input.Vorname;
            user.Abteilung = Input.Abteilung;
            // Achtung: Passwort sollte verschlüsselt werden!

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToPage("/Account/Login");
        }

        public class RegisterInput
        {
            public string Name { get; set; }
            public string Vorname { get; set; }
            public string Abteilung { get; set; }
            public string Passwort { get; set; }
        }
    }
} */