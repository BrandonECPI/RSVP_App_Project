using RSVPApp.Models;

namespace RSVPApp.Services;

/// <summary>
/// Hard-coded users and events for this week.
/// This will be replaced with local storage in Week 3.
/// </summary>
public static class SampleData
{
    // Hard-coded user accounts
    public static List<User> Users { get; } = new()
    {
        new User { Name = "Brandon Lazala", Email = "brandon@rsvp.com", Password = "Password1", Phone = "757-555-0101" },
        new User { Name = "Alex Rivera", Email = "alex@rsvp.com", Password = "Password1", Phone = "757-555-0102" }
    };

    // Hard-coded events (dates are based on today so deadlines stay valid)
    public static List<RsvpEvent> Events { get; } = new()
    {
        new RsvpEvent
        {
            HostName = "Brandon Lazala",
            Name = "Fall Game Night",
            Address = "123 Main St, Virginia Beach, VA",
            MaxAttendees = 10,
            EventDate = DateTime.Today.AddDays(10).AddHours(19),
            RsvpDeadline = DateTime.Today.AddDays(7),
            Attendees = new() { "Alex Rivera", "Jordan Lee" }
        },
        new RsvpEvent
        {
            HostName = "Alex Rivera",
            Name = "Mobile Dev Study Group",
            Address = "ECPI Library, Room 204",
            MaxAttendees = 8,
            EventDate = DateTime.Today.AddDays(5).AddHours(17),
            RsvpDeadline = DateTime.Today.AddDays(3),
            Attendees = new() { "Brandon Lazala", "Jordan Lee" }
        },
        new RsvpEvent
        {
            HostName = "Jordan Lee",
            Name = "Food Truck Friday",
            Address = "Town Center Plaza",
            MaxAttendees = 50,
            EventDate = DateTime.Today.AddDays(12).AddHours(12),
            RsvpDeadline = DateTime.Today.AddDays(10),
            Attendees = new() { "Brandon Lazala" }
        },
        new RsvpEvent
        {
            HostName = "Alex Rivera",
            Name = "Tech Meetup",
            Address = "456 Ocean Ave, Norfolk, VA",
            MaxAttendees = 2,
            EventDate = DateTime.Today.AddDays(20).AddHours(18),
            RsvpDeadline = DateTime.Today.AddDays(15),
            Attendees = new() { "Jordan Lee", "Sam Patel" }
        }
    };

    // Returns the matching user, or null if the email or password is wrong
    public static User? FindUser(string email, string password)
    {
        return Users.FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);
    }
}