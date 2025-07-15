/* using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AppManager.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Registration logic here
            return RedirectToPage("/Index");
        }

        public class InputModel
        {
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
    }
} */