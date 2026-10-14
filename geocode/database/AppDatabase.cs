using SQLite;
using geocode.models;

namespace geocode.database
{
    public class AppDatabase
    {
        private SQLiteAsyncConnection? _database;

        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (_database != null)
                return _database;

            _database = new SQLiteAsyncConnection(
                DatabaseConstants.DatabasePath,
                DatabaseConstants.Flags
            );

            await _database.ExecuteAsync(
                "PRAGMA foreign_keys = ON;"
            );

            return _database;
        }

        public async Task InitializeAsync()
        {
            var database = await GetConnectionAsync();

            await database.CreateTableAsync<Jornada>();

            await database.CreateTableAsync<Cajero>();

            await database.CreateTableAsync<ConfiguracionFacturacion>();

            await database.CreateTableAsync<JornadaCajero>();

            await database.CreateTableAsync<FacturaAsignada>();

            await database.CreateTableAsync<Domiciliario>();

            await database.CreateTableAsync<Entrega>();

            await database.CreateTableAsync<Bebida>();

            await database.CreateTableAsync<Inventario>();

            await database.CreateTableAsync<InventarioBebida>();

            await database.CreateTableAsync<MovimientoBebida>();

            await database.CreateTableAsync<EntregaBebida>();
        }
    }
}