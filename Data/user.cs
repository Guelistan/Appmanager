/* using System;
using System.ComponentModel.DataAnnotations;
using AppManager.Data;

namespace AppManager.Data
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public static implicit operator User(AppUser v)
        {
            throw new NotImplementedException();
        }
    }
}
 */