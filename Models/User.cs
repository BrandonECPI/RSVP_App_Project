namespace RSVPApp.Models;

/// <summary>
/// A registered user of the RSVP app.
/// </summary>
public class User
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}