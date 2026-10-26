using SQLite;

namespace RSVPApp.Models;

/// <summary>
/// One person attending one event. Stored in the Rsvps table.
/// </summary>
[Table("Rsvps")]
public class Rsvp
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Links to the event
    [Indexed]
    public int EventId { get; set; }

    // Links to the user (null for guests)
    public int? UserId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime RsvpDate { get; set; }
}