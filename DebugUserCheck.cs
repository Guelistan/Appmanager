using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppManager.Data;
using AppManager.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AppManager
{
    public class DebugUserCheck
    {
        public static async Task CheckUsersInDatabase(AppDbContext context, UserManager<AppUser> userManager)
        {
            Console.WriteLine("=== 🔍 DEBUG: Überprüfung der Benutzerdatenbank ===");
            Console.WriteLine();

            // Alle Benutzer aus der Datenbank abrufen
            var allUsers = await context.Users.ToListAsync();

            Console.WriteLine($"📊 Anzahl Benutzer in der Datenbank: {allUsers.Count}");
            Console.WriteLine();

            if (allUsers.Any())
            {
                Console.WriteLine("👥 Gefundene Benutzer:");
                foreach (var user in allUsers)
                {
                    Console.WriteLine($"  🔹 ID: {user.Id}");
                    Console.WriteLine($"     UserName: '{user.UserName}'");
                    Console.WriteLine($"     Email: '{user.Email}'");
                    Console.WriteLine($"     Vorname: '{user.Vorname}'");
                    Console.WriteLine($"     Nachname: '{user.Nachname}'");
                    Console.WriteLine($"     IsActive: {user.IsActive}");
                    Console.WriteLine($"     EmailConfirmed: {user.EmailConfirmed}");
                    Console.WriteLine($"     NormalizedUserName: '{user.NormalizedUserName}'");
                    Console.WriteLine($"     NormalizedEmail: '{user.NormalizedEmail}'");
                    Console.WriteLine();

                    // Test der Passwort-Verifikation
                    Console.WriteLine($"  🔐 Password Hash vorhanden: {!string.IsNullOrEmpty(user.PasswordHash)}");

                    // Test verschiedener Username-Varianten
                    var testUsernames = new[] { user.UserName, user.Email, user.NormalizedUserName };
                    Console.WriteLine($"  🧪 Mögliche Login-Varianten für diesen User:");

                    foreach (var testUsername in testUsernames.Where(u => !string.IsNullOrEmpty(u)).Distinct())
                    {
                        var foundByName = await userManager.FindByNameAsync(testUsername);
                        var foundByEmail = await userManager.FindByEmailAsync(testUsername);

                        Console.WriteLine($"     '{testUsername}' -> ByName: {foundByName != null}, ByEmail: {foundByEmail != null}");
                    }
                    Console.WriteLine("     " + new string('-', 50));
                }
            }
            else
            {
                Console.WriteLine("❌ Keine Benutzer in der Datenbank gefunden!");
            }

            Console.WriteLine();
            Console.WriteLine("🔧 Test-Suche für 'gülistan':");

            // Spezielle Tests für "gülistan"
            var searchTerms = new[] { "gülistan", "guelistan", "Gülistan", "GÜLISTAN" };

            foreach (var term in searchTerms)
            {
                var userByName = await userManager.FindByNameAsync(term);
                var userByEmail = await userManager.FindByEmailAsync(term);
                Console.WriteLine($"  '{term}' -> ByName: {userByName?.UserName ?? "nicht gefunden"}, ByEmail: {userByEmail?.UserName ?? "nicht gefunden"}");
            }

            Console.WriteLine();
            Console.WriteLine("=== Ende der Überprüfung ===");
        }
    }
}
