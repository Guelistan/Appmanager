using System;
using System.Collections.Generic;
using AppManager.Pages.Admin;
using AppManager.Models;

namespace AppManager.Models
{
    public class Application
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string ExecutablePath { get; set; }
        public bool IsStarted { get; set; }
        public bool RestartRequired { get; set; }
        public DateTime LastLaunchTime { get; set; }
        public string LastLaunchReason { get; set; }
        public List<AppLaunchHistory> LaunchHistory { get; set; } = new List<AppLaunchHistory>();
        public string Description { get; set; }
        public string IconPath { get; set; }
        public string Version { get; set; }
        public string Category { get; set; }
        public string Tags { get; set; } // Kommagetrennte Tags
        public string Path { get; set; } // Hinzugefügtes Feld
    }
}
