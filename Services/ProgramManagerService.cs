using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AppManager.Models;

namespace AppManager.Services
{
    public class ProgramManagerService
    {
        public Task<bool> StartProgramAsync(Application app) // ← async entfernt, da nicht verwendet
        {
            try
            {
                if (!File.Exists(app.ExecutablePath))
                {
                    return Task.FromResult(false);
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = app.ExecutablePath,
                    Arguments = app.Arguments,
                    WorkingDirectory = string.IsNullOrEmpty(app.WorkingDirectory)
                        ? Path.GetDirectoryName(app.ExecutablePath) ?? string.Empty
                        : app.WorkingDirectory,
                    UseShellExecute = true,
                    CreateNoWindow = false
                };

                if (app.RequiresAdmin)
                {
                    startInfo.Verb = "runas";
                }

                var process = Process.Start(startInfo);

                if (process != null)
                {
                    app.ProcessId = process.Id;
                    app.IsStarted = true;
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Starten von {app.Name}: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public Task<bool> StopProgramAsync(Application app) // ← async entfernt
        {
            try
            {
                if (app.ProcessId.HasValue)
                {
                    var process = Process.GetProcessById(app.ProcessId.Value);

                    if (!process.HasExited)
                    {
                        process.CloseMainWindow();

                        if (!process.WaitForExit(5000))
                        {
                            process.Kill();
                        }
                    }
                }

                app.IsStarted = false;
                app.ProcessId = null;
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Stoppen von {app.Name}: {ex.Message}");
                app.IsStarted = false;
                app.ProcessId = null;
                return Task.FromResult(false);
            }
        }

        public bool IsProgramRunning(Application app)
        {
            if (!app.ProcessId.HasValue) return false;

            try
            {
                var process = Process.GetProcessById(app.ProcessId.Value);
                return !process.HasExited;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RestartProgramAsync(Application app)
        {
            await StopProgramAsync(app);
            await Task.Delay(2000);
            return await StartProgramAsync(app);
        }
    }
}