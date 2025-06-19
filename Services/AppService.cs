using System.Diagnostics;
using AppManager.Models;

namespace AppManager.Services
{
    public class AppService
    {
        public void StartApp(Application app)
        {
            Process.Start(app.Path);
        }

        public void StopApp(Application app)
        {
            // Placeholder: stopping logic would depend on the app
        }
    }
}
