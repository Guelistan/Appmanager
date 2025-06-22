
using System;

// Repräsentiert einen Startverlaufseintrag
public class AppLaunchHistory
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public int UserId { get; set; }
    public DateTime LaunchTime { get; set; }
    public string Reason { get; set; }
}