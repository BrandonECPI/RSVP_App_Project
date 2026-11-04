using Microsoft.AspNetCore.Mvc;
using RSVPApi.DataAccess;
using RSVPApi.Models;

namespace RSVPApi.Controllers;

/// <summary>
/// RSVP endpoint: saves an RSVP if the deadline, capacity
/// plus duplicate rules all pass.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class RsvpsController : ControllerBase
{
    private readonly RsvpRepository repository = new();

    // POST: api/Rsvps
    [HttpPost]
    public IActionResult Create([FromBody] Rsvp rsvp)
    {
        string? error = repository.AddRsvp(rsvp);
        if (error != null)
        {
            return BadRequest(error);
        }

        return Ok();
    }
}