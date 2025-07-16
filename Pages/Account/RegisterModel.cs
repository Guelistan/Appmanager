using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using AppManager.Data;
using AppManager.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AppManager.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender; // Du brauchst eine Implementierung

        public RegisterModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new();

        public class RegisterInput
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Name { get; set; }

            [Required]
            public string Vorname { get; set; }

            [Required]
            public string Abteilung { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Passwort { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = new AppUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                Vorname = Input.Vorname,
                Abteilung = Input.Abteilung,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, Input.Passwort);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = user.Id, token = token },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Bestätige deine E-Mail",
                    $"Bitte bestätige dein Konto durch Klicken auf diesen Link: <a href='{confirmationUrl}'>Bestätigen</a>");

                return RedirectToPage("/Account/Login", new { Message = "Registrierung erfolgreich. Bitte bestätige deine E-Mail." });
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return Page();
        }
    }
}
