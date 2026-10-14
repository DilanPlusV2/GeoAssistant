using geocode.models;
using SQLite;

namespace geocode.services
{
    public class DatabaseSeedService
    {
        private readonly DatabaseService _databaseService;

        public DatabaseSeedService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task SeedAsync()
        {
            var database =
                await _databaseService.GetConnectionAsync();

            await SeedCajerosAsync(database);
            await SeedConfiguracionesAsync(database);
            await SeedDomiciliariosAsync(database);
            await SeedBebidasAsync(database);
        }

        // =========================================================
        // CAJEROS
        // =========================================================

        private async Task SeedCajerosAsync(
            SQLiteAsyncConnection database)
        {
            var cantidad =
                await database.Table<Cajero>().CountAsync();

            if (cantidad > 0)
                return;

            var cajeros = new List<Cajero>
            {
                new()
                {
                    Nombre = "Cajero 1",
                    Activo = true
                },

                new()
                {
                    Nombre = "Keylis",
                    Activo = true
                },

                new()
                {
                    Nombre = "Cajero 3",
                    Activo = true
                },

                new()
                {
                    Nombre = "Maria Jose",
                    Activo = true
                }
            };

            await database.InsertAllAsync(cajeros);
        }

        // =========================================================
        // CONFIGURACIONES DE FACTURACIÓN
        // =========================================================

        private async Task SeedConfiguracionesAsync(
            SQLiteAsyncConnection database)
        {
            var cantidad =
                await database
                    .Table<ConfiguracionFacturacion>()
                    .CountAsync();

            if (cantidad > 0)
                return;

            var cajeros =
                await database
                    .Table<Cajero>()
                    .ToListAsync();

            var cajero1 = cajeros
                .FirstOrDefault(x => x.Nombre == "Cajero 1");

            var keylis = cajeros
                .FirstOrDefault(x => x.Nombre == "Keylis");

            var cajero3 = cajeros
                .FirstOrDefault(x => x.Nombre == "Cajero 3");

            var mariaJose = cajeros
                .FirstOrDefault(x => x.Nombre == "Maria Jose");

            var configuraciones =
                new List<ConfiguracionFacturacion>();

            if (cajero1 != null)
            {
                configuraciones.Add(
                    new ConfiguracionFacturacion
                    {
                        CajeroId = cajero1.Id,
                        Serie = "000",
                        Tipo = "NORMAL",
                        Activa = true
                    }
                );

                configuraciones.Add(
                    new ConfiguracionFacturacion
                    {
                        CajeroId = cajero1.Id,
                        Serie = "000",
                        Tipo = "ELECTRONICA",
                        Activa = true
                    }
                );
            }

            if (keylis != null)
            {
                configuraciones.Add(
                    new ConfiguracionFacturacion
                    {
                        CajeroId = keylis.Id,
                        Serie = "006",
                        Tipo = "NORMAL",
                        Activa = true
                    }
                );

                configuraciones.Add(
                    new ConfiguracionFacturacion
                    {
                        CajeroId = keylis.Id,
                        Serie = "006",
                        Tipo = "ELECTRONICA",
                        Activa = true
                    }
                );
            }

            if (cajero3 != null)
            {
                configuraciones.Add(
                    new ConfiguracionFacturacion
                    {
                        CajeroId = cajero3.Id,
                        Serie = "007",
                        Tipo = "NORMAL",
                        Activa = true
                    }
                );

                configuraciones.Add(
                    new ConfiguracionFacturacion
                    {
                        CajeroId = cajero3.Id,
                        Serie = "007",
                        Tipo = "ELECTRONICA",
                        Activa = true
                    }
                );
            }

            if (mariaJose != null)
            {
                configuraciones.Add(
                    new ConfiguracionFacturacion
                    {
                        CajeroId = mariaJose.Id,
                        Serie = "008",
                        Tipo = "NORMAL",
                        Activa = true
                    }
                );

                configuraciones.Add(
                    new ConfiguracionFacturacion
                    {
                        CajeroId = mariaJose.Id,
                        Serie = "008",
                        Tipo = "ELECTRONICA",
                        Activa = true
                    }
                );
            }

            await database.InsertAllAsync(configuraciones);
        }

        // =========================================================
        // DOMICILIARIOS
        // =========================================================

        private async Task SeedDomiciliariosAsync(
            SQLiteAsyncConnection database)
        {
            var cantidad =
                await database
                    .Table<Domiciliario>()
                    .CountAsync();

            if (cantidad > 0)
                return;

            var codigos = new[]
            {
                "A",
                "C",
                "D",
                "E",
                "F",
                "J",
                "K",
                "L",
                "M",
                "P",
                "R",
                "S",
                "V",
                "W",
                "Z"
            };

            var domiciliarios = codigos
                .Select(codigo => new Domiciliario
                {
                    Codigo = codigo,
                    Activo = true
                })
                .ToList();

            await database.InsertAllAsync(
                domiciliarios
            );
        }

        // =========================================================
        // BEBIDAS
        // =========================================================

        private async Task SeedBebidasAsync(
            SQLiteAsyncConnection database)
        {
            var cantidad =
                await database
                    .Table<Bebida>()
                    .CountAsync();

            if (cantidad > 0)
                return;

            var bebidas = new List<Bebida>
            {
                new()
                {
                    Nombre = "Coca-Cola",
                    Activa = true
                },

                new()
                {
                    Nombre = "Agua",
                    Activa = true
                }
            };

            await database.InsertAllAsync(bebidas);
        }
    }
}