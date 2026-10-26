using SQLite;
using RSVPApp.Models;

namespace RSVPApp.DataAccess;

/// <summary>
/// Data access layer. All reads and saves for users, events
/// and RSVPs go through this class.
/// </summary>
public class RsvpDatabase
{
    // One shared copy used by every page
    public static RsvpDatabase Instance { get; } = new();

    // Connection to the SQLite database (null until first use)
    private SQLiteAsyncConnection? database;

    /// <summary>
    /// Opens the database, creates the tables, then adds sample data on first run.
    /// </summary>
    private async Task Init()
    {
        if (database is not null)
        {
            return;
        }

        var db = new SQLiteAsyncConnection(DatabaseConstants.DatabasePath, DatabaseConstants.Flags);
        await db.CreateTableAsync<User>();
        await db.CreateTableAsync<RsvpEvent>();
        await db.CreateTableAsync<Rsvp>();

        database = db;
        await SeedAsync();
    }

    /// <summary>
    /// Adds starter users, events and RSVPs if the database is empty.
    /// </summary>
    private async Task SeedAsync()
    {
        if (await database!.Table<User>().CountAsync() > 0)
        {
            return;
        }

        var brandon = new User { Name = "Brandon Lazala", Email = "brandon@rsvp.com", Password = "Password1", Phone = "757-555-0101" };
        var alex = new User { Name = "Alex Rivera", Email = "alex@rsvp.com", Password = "Password1", Phone = "757-555-0102" };
        await database.InsertAsync(brandon);
        await database.InsertAsync(alex);

        var gameNight = new RsvpEvent
        {
            HostUserId = brandon.Id,
            HostName = brandon.Name,
            Name = "Fall Game Night",
            Address = "123 Main St, Virginia Beach, VA",
            MaxAttendees = 10,
            EventDate = DateTime.Today.AddDays(10).AddHours(19),
            RsvpDeadline = DateTime.Today.AddDays(7)
        };
        var studyGroup = new RsvpEvent
        {
            HostUserId = alex.Id,
            HostName = alex.Name,
            Name = "Mobile Dev Study Group",
            Address = "ECPI Library, Room 204",
            MaxAttendees = 8,
            EventDate = DateTime.Today.AddDays(5).AddHours(17),
            RsvpDeadline = DateTime.Today.AddDays(3)
        };
        var techMeetup = new RsvpEvent
        {
            HostUserId = alex.Id,
            HostName = alex.Name,
            Name = "Tech Meetup",
            Address = "456 Ocean Ave, Norfolk, VA",
            MaxAttendees = 20,
            EventDate = DateTime.Today.AddDays(20).AddHours(18),
            RsvpDeadline = DateTime.Today.AddDays(15)
        };
        await database.InsertAsync(gameNight);
        await database.InsertAsync(studyGroup);
        await database.InsertAsync(techMeetup);

        // Alex is going to Brandon's event; Brandon is going to Alex's study group
        await database.InsertAsync(new Rsvp { EventId = gameNight.Id, UserId = alex.Id, Name = alex.Name, Email = alex.Email, Phone = alex.Phone, RsvpDate = DateTime.Now });
        await database.InsertAsync(new Rsvp { EventId = studyGroup.Id, UserId = brandon.Id, Name = brandon.Name, Email = brandon.Email, Phone = brandon.Phone, RsvpDate = DateTime.Now });
    }

    // ---------- Users ----------

    /// <summary>
    /// Input: email and password. Output: the matching user, or null if login fails.
    /// </summary>
    public async Task<User?> LoginAsync(string email, string password)
    {
        await Init();
        string cleanEmail = email.Trim().ToLowerInvariant();
        return await database!.Table<User>()
            .Where(u => u.Email == cleanEmail && u.Password == password)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Saves a new user. Returns false if the email is already taken.
    /// </summary>
    public async Task<bool> AddUserAsync(User user)
    {
        await Init();
        user.Email = user.Email.Trim().ToLowerInvariant();
        string email = user.Email;

        int count = await database!.Table<User>().Where(u => u.Email == email).CountAsync();
        if (count > 0)
        {
            return false;
        }

        await database.InsertAsync(user);
        return true;
    }

    // ---------- Events ----------

    /// <summary>
    /// Output: every event, soonest first.
    /// </summary>
    public async Task<List<RsvpEvent>> GetAllEventsAsync()
    {
        await Init();
        return await database!.Table<RsvpEvent>().OrderBy(ev => ev.EventDate).ToListAsync();
    }

    /// <summary>
    /// Output: events the given user is hosting.
    /// </summary>
    public async Task<List<RsvpEvent>> GetHostingEventsAsync(int userId)
    {
        await Init();
        return await database!.Table<RsvpEvent>()
            .Where(ev => ev.HostUserId == userId)
            .OrderBy(ev => ev.EventDate)
            .ToListAsync();
    }

    /// <summary>
    /// Output: events the given user has RSVP'd to.
    /// </summary>
    public async Task<List<RsvpEvent>> GetAttendingEventsAsync(int userId)
    {
        await Init();
        int? id = userId;
        List<Rsvp> rsvps = await database!.Table<Rsvp>().Where(r => r.UserId == id).ToListAsync();
        HashSet<int> eventIds = rsvps.Select(r => r.EventId).ToHashSet();

        List<RsvpEvent> allEvents = await GetAllEventsAsync();
        return allEvents.Where(ev => eventIds.Contains(ev.Id)).ToList();
    }

    /// <summary>
    /// Saves a new event.
    /// </summary>
    public async Task<int> AddEventAsync(RsvpEvent newEvent)
    {
        await Init();
        return await database!.InsertAsync(newEvent);
    }

    // ---------- RSVPs ----------

    /// <summary>
    /// Output: the names of everyone attending the given event.
    /// </summary>
    public async Task<List<string>> GetAttendeeNamesAsync(int eventId)
    {
        await Init();
        List<Rsvp> rsvps = await database!.Table<Rsvp>().Where(r => r.EventId == eventId).ToListAsync();
        return rsvps.Select(r => r.Name).ToList();
    }

    /// <summary>
    /// Returns true if this user (or email, for guests) already RSVP'd to the event.
    /// </summary>
    public async Task<bool> HasRsvpAsync(int eventId, int? userId, string email)
    {
        await Init();
        string cleanEmail = email.Trim().ToLowerInvariant();
        List<Rsvp> rsvps = await database!.Table<Rsvp>().Where(r => r.EventId == eventId).ToListAsync();
        return rsvps.Any(r => (userId != null && r.UserId == userId) || r.Email == cleanEmail);
    }

    /// <summary>
    /// Saves a new RSVP.
    /// </summary>
    public async Task<int> AddRsvpAsync(Rsvp rsvp)
    {
        await Init();
        rsvp.Email = rsvp.Email.Trim().ToLowerInvariant();
        return await database!.InsertAsync(rsvp);
    }
}