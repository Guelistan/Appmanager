using System;
using System.Collections.Generic;

namespace AppManager.Models
{
    // Repräsentiert eine Anwendung
    public class Application
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ExecutablePath { get; set; }
        public List<User> Admins { get; set; } = new List<User>();
        public List<User> AllowedUsers { get; set; } = new List<User>();
        public string Path { get; set; } // Hinzugefügtes Feld
    }

    // Repräsentiert einen Benutzer
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public bool IsAdmin { get; set; }
    }

    // Repräsentiert einen Startverlaufseintrag
    public class AppLaunchHistory
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int UserId { get; set; }
        public DateTime LaunchTime { get; set; }
        public string Reason { get; set; }
    }
}
