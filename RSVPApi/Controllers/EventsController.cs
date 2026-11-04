using Microsoft.AspNetCore.Mvc;
using RSVPApi.DataAccess;
using RSVPApi.Models;

namespace RSVPApi.Controllers;

/// <summary>
/// Event endpoints: list events, add an event, plus get attendee names.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly RsvpRepository repository = new();

    // GET: api/Events — all events
    [HttpGet]
    public IEnumerable<RsvpEvent> GetAll()
    {
        return repository.GetAllEvents();
    }

    // GET: api/Events/hosting/1 — events the user is hosting
    [HttpGet("hosting/{userId:int}")]
    public IEnumerable<RsvpEvent> GetHosting(int userId)
    {
        return repository.GetHostingEvents(userId);
    }

    // GET: api/Events/attending/1 — events the user is attending
    [HttpGet("attending/{userId:int}")]
    public IEnumerable<RsvpEvent> GetAttending(int userId)
    {
        return repository.GetAttendingEvents(userId);
    }

    // GET: api/Events/1/attendees — names of everyone attending the event
    [HttpGet("{eventId:int}/attendees")]
    public IEnumerable<string> GetAttendees(int eventId)
    {
        return repository.GetAttendeeNames(eventId);
    }

    // POST: api/Events — adds a new event
    [HttpPost]
    public IActionResult Create([FromBody] RsvpEvent newEvent)
    {
        repository.AddEvent(newEvent);
        return Ok(newEvent);
    }
}