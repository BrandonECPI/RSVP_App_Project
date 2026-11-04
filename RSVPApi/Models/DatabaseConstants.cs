namespace RSVPApi.Models;

/// <summary>
/// Database settings: file name, open options and file location.
/// </summary>
public static class DatabaseConstants
{
    public const string DatabaseFilename = "RsvpApi.db3";

    public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite
        // create the database if it doesn't exist
        | SQLite.SQLiteOpenFlags.Create
        // allow safe use from more than one request at a time
        | SQLite.SQLiteOpenFlags.FullMutex
        // enable multi-threading
        | SQLite.SQLiteOpenFlags.SharedCache;

    // Database file is stored next to the running web API
    public static string DatabasePath =>
        Path.Combine(AppContext.BaseDirectory, DatabaseFilename);
}