using Microsoft.AspNetCore.Mvc;
using RSVPApi.DataAccess;
using RSVPApi.Models;

namespace RSVPApi.Controllers;

/// <summary>
/// User endpoints: log in (Basic Authentication) plus create an account.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly RsvpRepository repository = new();

    // GET: api/Users/login — returns the user if the Basic Authentication login is valid
    [HttpGet("login")]
    [BasicAuthentication]
    public ActionResult<User> Login()
    {
        var user = (User)HttpContext.Items["AuthUser"]!;

        // Send the user back without the password
        return Ok(new User
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Password = string.Empty
        });
    }

    // POST: api/Users — creates a new account
    [HttpPost]
    public IActionResult Create([FromBody] User user)
    {
        if (!repository.AddUser(user))
        {
            return Conflict("An account with that email already exists.");
        }

        return Ok();
    }
}