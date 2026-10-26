using SQLite;

namespace RSVPApp.Models;

/// <summary>
/// An event users can RSVP for. Stored in the Events table.
/// </summary>
[Table("Events")]
public class RsvpEvent
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Links to the user who created the event
    [Indexed]
    public int HostUserId { get; set; }

    public string HostName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int MaxAttendees { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime RsvpDeadline { get; set; }

    // Display helpers (not saved in the database)
    [Ignore]
    public string DateDisplay => EventDate.ToString("ddd, MMM d, yyyy 'at' h:mm tt");

    [Ignore]
    public string HostDisplay => $"Hosted by {HostName}";
}