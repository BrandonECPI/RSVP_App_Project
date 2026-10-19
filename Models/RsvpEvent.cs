namespace RSVPApp.Models;

/// <summary>
/// An event that users can view or RSVP for.
/// </summary>
public class RsvpEvent
{
    public string HostName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int MaxAttendees { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime RsvpDeadline { get; set; }

    // Names of the people attending this event
    public List<string> Attendees { get; set; } = new();

    // Read-only helpers used for display on the screens
    public int AttendeeCount => Attendees.Count;
    public string DateDisplay => EventDate.ToString("ddd, MMM d, yyyy 'at' h:mm tt");
    public string HostDisplay => $"Hosted by {HostName}";
}