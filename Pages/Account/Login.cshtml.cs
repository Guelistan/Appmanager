using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using AppManager.Data;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace AppManager.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public LoginModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public LoginInput Input { get; set; }

        public string Message { get; set; }

        public class LoginInput
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet(string message = null)
        {
            if (!string.IsNullOrEmpty(message))
                Message = message;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.FindByNameAsync(Input.Username);
            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Ungültige Anmeldedaten oder Benutzer ist inaktiv.");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToPage("/Admin/Dashboard");
            }

            ModelState.AddModelError(string.Empty, "Anmeldung fehlgeschlagen.");
            return Page();
        }
    }
}
