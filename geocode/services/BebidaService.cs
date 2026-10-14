using geocode.models;

namespace geocode.services
{
    public class BebidaService
    {
        private readonly DatabaseService _databaseService;

        public BebidaService(
            DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // =========================================================
        // CREAR BEBIDA
        // =========================================================

        public async Task<Bebida> CrearAsync(
            string nombre)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            var nombreNormalizado =
                nombre.Trim();

            var existente = await database
                .Table<Bebida>()
                .Where(x =>
                    x.Nombre == nombreNormalizado)
                .FirstOrDefaultAsync();

            if (existente != null)
                return existente;

            var bebida = new Bebida
            {
                Nombre = nombreNormalizado,
                Activa = true
            };

            await database.InsertAsync(bebida);

            return bebida;
        }

        // =========================================================
        // OBTENER POR ID
        // =========================================================

        public async Task<Bebida?> ObtenerPorIdAsync(
            int id)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<Bebida>()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // OBTENER POR NOMBRE
        // =========================================================

        public async Task<Bebida?> ObtenerPorNombreAsync(
            string nombre)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            var nombreNormalizado =
                nombre.Trim();

            return await database
                .Table<Bebida>()
                .Where(x =>
                    x.Nombre == nombreNormalizado &&
                    x.Activa)
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // OBTENER BEBIDAS ACTIVAS
        // =========================================================

        public async Task<List<Bebida>>
            ObtenerActivasAsync()
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<Bebida>()
                .Where(x => x.Activa)
                .OrderBy(x => x.Nombre)
                .ToListAsync();
        }

        // =========================================================
        // DESACTIVAR
        // =========================================================

        public async Task DesactivarAsync(
            Bebida bebida)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            bebida.Activa = false;

            await database.UpdateAsync(bebida);
        }
    }
}