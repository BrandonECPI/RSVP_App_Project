using RSVPApp.Models;

namespace RSVPApp.Services;

/// <summary>
/// Tracks who is logged in. A null user means the person is a guest.
/// </summary>
public static class Session
{
    public static User? CurrentUser { get; set; }

    public static bool IsGuest => CurrentUser == null;

    // Clears the logged-in user
    public static void Logout()
    {
        CurrentUser = null;
    }
}