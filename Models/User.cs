using SQLite;

namespace RSVPApp.Models;

/// <summary>
/// A registered user. Stored in the Users table.
/// </summary>
[Table("Users")]
public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    // Unique so two accounts can't share an email
    [Unique]
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}