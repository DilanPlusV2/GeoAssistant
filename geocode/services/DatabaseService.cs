using SQLite;
using geocode.database;

namespace geocode.services
{
    public class DatabaseService
    {
        private readonly AppDatabase _appDatabase;

        public DatabaseService()
        {
            _appDatabase = new AppDatabase();
        }

        public Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            return _appDatabase.GetConnectionAsync();
        }

        public Task InitializeAsync()
        {
            return _appDatabase.InitializeAsync();
        }
    }
}