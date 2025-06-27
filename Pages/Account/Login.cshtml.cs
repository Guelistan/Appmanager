using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace AppManager.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginInput Input { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            // Beispiel: Dummy-Login
            if (Input.Name == "admin" && Input.Passwort == "passwort")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Input.Name),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                HttpContext.SignInAsync("MyCookieAuth", principal).Wait();

                return RedirectToPage("/Index");
            }

            ModelState.AddModelError("", "Login fehlgeschlagen");
            return Page();
        }

        public class LoginInput
        {
            public string Name { get; set; }
            public string Vorname { get; set; }
            public string Abteilung { get; set; }
            public string Passwort { get; set; }
        }
        
    }
}