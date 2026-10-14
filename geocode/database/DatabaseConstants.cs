using SQLite;

namespace geocode.database
{
    public static class DatabaseConstants
    {
        public const string DatabaseFilename = "geocode.db3";

        public const SQLiteOpenFlags Flags =
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(
                FileSystem.AppDataDirectory,
                DatabaseFilename
            );
    }
}