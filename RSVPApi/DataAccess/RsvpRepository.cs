using SQLite;
using RSVPApi.Models;

namespace RSVPApi.DataAccess;

/// <summary>
/// Data access layer for the API. All database reads plus saves
/// for users, events and RSVPs go through this class.
/// </summary>
public class RsvpRepository
{
    // One shared connection for the whole API
    private static readonly object initLock = new();
    private static SQLiteConnection? database;

    // Opens the database, creates the tables, then adds sample data on first run
    private static SQLiteConnection Db
    {
        get
        {
            lock (initLock)
            {
                if (database is null)
                {
                    var db = new SQLiteConnection(DatabaseConstants.DatabasePath, DatabaseConstants.Flags);
                    db.CreateTable<User>();
                    db.CreateTable<RsvpEvent>();
                    db.CreateTable<Rsvp>();
                    Seed(db);
                    database = db;
                }

                return database;
            }
        }
    }

    /// <summary>
    /// Adds starter users, events plus RSVPs if the database is empty.
    /// </summary>
    private static void Seed(SQLiteConnection db)
    {
        if (db.Table<User>().Count() > 0)
        {
            return;
        }

        var brandon = new User { Name = "Brandon Lazala", Email = "brandon@rsvp.com", Password = "Password1", Phone = "757-555-0101" };
        var alex = new User { Name = "Alex Rivera", Email = "alex@rsvp.com", Password = "Password1", Phone = "757-555-0102" };
        db.Insert(brandon);
        db.Insert(alex);

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
        db.Insert(gameNight);
        db.Insert(studyGroup);
        db.Insert(techMeetup);

        // Alex is going to Brandon's event; Brandon is going to Alex's study group
        db.Insert(new Rsvp { EventId = gameNight.Id, UserId = alex.Id, Name = alex.Name, Email = alex.Email, Phone = alex.Phone, RsvpDate = DateTime.Now });
        db.Insert(new Rsvp { EventId = studyGroup.Id, UserId = brandon.Id, Name = brandon.Name, Email = brandon.Email, Phone = brandon.Phone, RsvpDate = DateTime.Now });
    }

    // ---------- Users ----------

    /// <summary>
    /// Input: email plus password. Output: the matching user, or null if the login is wrong.
    /// </summary>
    public User? ValidateUser(string email, string password)
    {
        string cleanEmail = email.Trim().ToLowerInvariant();
        return Db.Table<User>()
            .Where(u => u.Email == cleanEmail && u.Password == password)
            .FirstOrDefault();
    }

    /// <summary>
    /// Saves a new user. Returns false if the email is already taken.
    /// </summary>
    public bool AddUser(User user)
    {
        user.Email = user.Email.Trim().ToLowerInvariant();
        string email = user.Email;

        if (Db.Table<User>().Where(u => u.Email == email).Count() > 0)
        {
            return false;
        }

        user.Id = 0;
        Db.Insert(user);
        return true;
    }

    // ---------- Events ----------

    /// <summary>
    /// Output: every event, soonest first.
    /// </summary>
    public List<RsvpEvent> GetAllEvents()
    {
        return Db.Table<RsvpEvent>().OrderBy(ev => ev.EventDate).ToList();
    }

    /// <summary>
    /// Output: events the given user is hosting.
    /// </summary>
    public List<RsvpEvent> GetHostingEvents(int userId)
    {
        return Db.Table<RsvpEvent>()
            .Where(ev => ev.HostUserId == userId)
            .OrderBy(ev => ev.EventDate)
            .ToList();
    }

    /// <summary>
    /// Output: events the given user has RSVP'd to.
    /// </summary>
    public List<RsvpEvent> GetAttendingEvents(int userId)
    {
        int? id = userId;
        HashSet<int> eventIds = Db.Table<Rsvp>()
            .Where(r => r.UserId == id)
            .ToList()
            .Select(r => r.EventId)
            .ToHashSet();

        return GetAllEvents().Where(ev => eventIds.Contains(ev.Id)).ToList();
    }

    /// <summary>
    /// Output: one event by ID, or null if it doesn't exist.
    /// </summary>
    public RsvpEvent? GetEvent(int eventId)
    {
        return Db.Table<RsvpEvent>().Where(ev => ev.Id == eventId).FirstOrDefault();
    }

    /// <summary>
    /// Saves a new event.
    /// </summary>
    public void AddEvent(RsvpEvent newEvent)
    {
        newEvent.Id = 0;
        Db.Insert(newEvent);
    }

    // ---------- RSVPs ----------

    /// <summary>
    /// Output: the names of everyone attending the given event.
    /// </summary>
    public List<string> GetAttendeeNames(int eventId)
    {
        return Db.Table<Rsvp>()
            .Where(r => r.EventId == eventId)
            .ToList()
            .Select(r => r.Name)
            .ToList();
    }

    /// <summary>
    /// Saves an RSVP if every rule passes.
    /// Output: null if saved, or an error message explaining why it wasn't.
    /// </summary>
    public string? AddRsvp(Rsvp rsvp)
    {
        RsvpEvent? rsvpEvent = GetEvent(rsvp.EventId);
        if (rsvpEvent == null)
        {
            return "That event could not be found.";
        }

        // Rule 1: today must be on or before the RSVP deadline
        if (DateTime.Today > rsvpEvent.RsvpDeadline.Date)
        {
            return "The RSVP deadline has passed.";
        }

        int eventId = rsvpEvent.Id;
        List<Rsvp> existing = Db.Table<Rsvp>().Where(r => r.EventId == eventId).ToList();

        // Rule 2: the event can't be full
        if (existing.Count >= rsvpEvent.MaxAttendees)
        {
            return "This event is full.";
        }

        // Rule 3: no double RSVPs (checked by user ID, or by email for guests)
        string cleanEmail = rsvp.Email.Trim().ToLowerInvariant();
        bool alreadyRsvpd = existing.Any(r =>
            (rsvp.UserId != null && r.UserId == rsvp.UserId) || r.Email == cleanEmail);

        if (alreadyRsvpd)
        {
            return "You have already RSVP'd for this event.";
        }

        rsvp.Id = 0;
        rsvp.Email = cleanEmail;
        rsvp.RsvpDate = DateTime.Now;
        Db.Insert(rsvp);
        return null;
    }
}