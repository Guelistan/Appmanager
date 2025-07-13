
using System;
namespace AppManager.Models
{
    // Repräsentiert einen Benutzer
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    // Repräsentiert einen Startverlaufseintrag
    public class AppLaunchHistory
    {
        public int? Id { get; set; }
        public int? ApplicationId { get; set; }
        public int? UserId { get; set; }
        public DateTime LaunchTime { get; set; }
        public string Reason { get; set; }
    }
}