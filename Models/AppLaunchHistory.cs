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

        public string Action { get; set; } = string.Empty;

        public AppUser? User { get; set; }

        public DateTime LaunchTime { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}
