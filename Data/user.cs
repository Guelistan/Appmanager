
// Repräsentiert einen Benutzer
namespace AppManager.Data
{
    
public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public bool IsAdmin { get; set; }
    }
}