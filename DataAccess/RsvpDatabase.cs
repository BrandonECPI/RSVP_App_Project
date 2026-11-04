using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using RSVPApp.Models;

namespace RSVPApp.DataAccess;

/// <summary>
/// Data access layer. As of Week 4, this talks to the RSVP web API
/// instead of a local database. Method names stayed the same,
/// so the pages didn't need to change.
/// </summary>
public class RsvpDatabase
{
    // One shared copy used by every page
    public static RsvpDatabase Instance { get; } = new();

    // Web API address (change the port to match your running API)
    private const string ApiBaseUrl = "http://localhost:5035/";

    // One shared HttpClient for every request
    private static readonly HttpClient client = new() { BaseAddress = new Uri(ApiBaseUrl) };

    // ---------- Users ----------

    /// <summary>
    /// Sends the login to the API using Basic Authentication.
    /// Output: the user if the login works, or null if it doesn't.
    /// </summary>
    public async Task<User?> LoginAsync(string email, string password)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/Users/login");

        // Build the "Basic <base64 email:password>" header
        string token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email.Trim()}:{password}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);

        HttpResponseMessage response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<User>();
    }

    /// <summary>
    /// Creates a new account. Returns false if the email is already taken.
    /// </summary>
    public async Task<bool> AddUserAsync(User user)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/Users", user);
        return response.IsSuccessStatusCode;
    }

    // ---------- Events ----------

    // Output: every event
    public async Task<List<RsvpEvent>> GetAllEventsAsync()
    {
        return await client.GetFromJsonAsync<List<RsvpEvent>>("api/Events") ?? new List<RsvpEvent>();
    }

    // Output: events the given user is hosting
    public async Task<List<RsvpEvent>> GetHostingEventsAsync(int userId)
    {
        return await client.GetFromJsonAsync<List<RsvpEvent>>($"api/Events/hosting/{userId}") ?? new List<RsvpEvent>();
    }

    // Output: events the given user is attending
    public async Task<List<RsvpEvent>> GetAttendingEventsAsync(int userId)
    {
        return await client.GetFromJsonAsync<List<RsvpEvent>>($"api/Events/attending/{userId}") ?? new List<RsvpEvent>();
    }

    // Saves a new event. Output: 1 if saved, 0 if not.
    public async Task<int> AddEventAsync(RsvpEvent newEvent)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/Events", newEvent);
        return response.IsSuccessStatusCode ? 1 : 0;
    }

    // ---------- RSVPs ----------

    // Output: the names of everyone attending the given event
    public async Task<List<string>> GetAttendeeNamesAsync(int eventId)
    {
        return await client.GetFromJsonAsync<List<string>>($"api/Events/{eventId}/attendees") ?? new List<string>();
    }

    /// <summary>
    /// Sends an RSVP to the API. The API checks the deadline, capacity plus duplicates.
    /// Output: null if saved, or the error message from the API.
    /// </summary>
    public async Task<string?> AddRsvpAsync(Rsvp rsvp)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/Rsvps", rsvp);
        if (response.IsSuccessStatusCode)
        {
            return null;
        }

        string message = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(message) ? "The RSVP could not be saved." : message.Trim('"');
    }
}