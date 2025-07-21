using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppManager.Models
{
    public class Application
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsStarted { get; set; } = false;

        public bool RestartRequired { get; set; } = false;

        // ✨ NEU: Pfad zur ausführbaren Datei
        [Required]
        public string ExecutablePath { get; set; } = string.Empty;

        // ✨ NEU: Process-ID des laufenden Programms
        public int? ProcessId { get; set; }

        // ✨ NEU: Arbeitsverzeichnis (optional)
        public string WorkingDirectory { get; set; } = string.Empty;

        // ✨ NEU: Kommandozeilen-Argumente (optional)
        public string Arguments { get; set; } = string.Empty;

        // ✨ NEU: Sicherheitslevel
        public bool RequiresAdmin { get; set; } = false;

        public DateTime LastLaunchTime { get; set; }
        public string LastLaunchReason { get; set; }
        public List<AppLaunchHistory> LaunchHistory { get; set; } = new List<AppLaunchHistory>();
        public string IconPath { get; set; }
        public string Version { get; set; }
        public string Category { get; set; }
        public string Tags { get; set; } // Kommagetrennte Tags
        public string Path { get; set; } // Hinzugefügtes Feld
    }
}
