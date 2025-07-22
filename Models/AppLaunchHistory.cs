#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using AppManager.Data;

namespace AppManager.Models
{
    public class AppLaunchHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid ApplicationId { get; set; }
        
        public Application? Application { get; set; }

        public string UserId { get; set; } = string.Empty;
        
        public DateTime LaunchTime { get; set; } = DateTime.Now;
        
        public string Action { get; set; } = string.Empty; // "Start", "Stop", "Restart"
        
        public string Reason { get; set; } = string.Empty;
        
        // Navigation Properties
        public virtual AppUser? User { get; set; }
    }
}
