namespace RSVPApp.Models;

/// <summary>
/// Database settings: file name, open options and file location.
/// </summary>
public static class DatabaseConstants
{
    public const string DatabaseFilename = "RsvpApp.db3";

    public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite
        // create the database if it doesn't exist
        | SQLite.SQLiteOpenFlags.Create
        // enable multi-threading
        | SQLite.SQLiteOpenFlags.SharedCache;

    // Full path to the database file in the app's data folder
    public static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
}