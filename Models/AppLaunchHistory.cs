using System;
using AppManager.Data;
using AppManager.Models;

namespace AppManager.Models
{
    public class AppLaunchHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // ✔ Verbindlich (nicht-nullable) GUID
        public Guid ApplicationId { get; set; }

        public Application Application { get; set; }

        // ✔ String UserId reicht (wenn Identity verwendet wird)
        public string UserId { get; set; }

        public AppUser User { get; set; }

        public DateTime LaunchTime { get; set; }

        public string Reason { get; set; }
    }
}
