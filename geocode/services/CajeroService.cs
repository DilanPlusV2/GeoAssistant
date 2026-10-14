using geocode.models;

namespace geocode.services
{
    public class CajeroService
    {
        private readonly DatabaseService _databaseService;

        public CajeroService(
            DatabaseService databaseService)
        {
            _databaseService =
                databaseService;
        }


        // =========================================================
        // OBTENER ACTIVOS
        // =========================================================

        public async Task<List<Cajero>>
            ObtenerTodosAsync()
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();

            return await database
                .Table<Cajero>()
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .ToListAsync();
        }


        // =========================================================
        // OBTENER TODOS
        // =========================================================

        public async Task<List<Cajero>>
            ObtenerTodosIncluidosInactivosAsync()
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();

            return await database
                .Table<Cajero>()
                .OrderBy(x => x.Nombre)
                .ToListAsync();
        }


        // =========================================================
        // OBTENER POR ID
        // =========================================================

        public async Task<Cajero?>
            ObtenerPorIdAsync(int id)
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();

            return await database
                .Table<Cajero>()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }


        // =========================================================
        // CREAR
        // =========================================================

        public async Task<Cajero>
            CrearAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException(
                    "El nombre del cajero es obligatorio.");
            }


            var database =
                await _databaseService
                    .GetConnectionAsync();


            var nombreLimpio =
                nombre.Trim();


            var existente =
                await database
                    .Table<Cajero>()
                    .Where(x =>
                        x.Nombre == nombreLimpio)
                    .FirstOrDefaultAsync();


            if (existente != null)
            {
                throw new InvalidOperationException(
                    "Ya existe un cajero con ese nombre.");
            }


            var cajero =
                new Cajero
                {
                    Nombre =
                        nombreLimpio,

                    Activo = true
                };


            await database
                .InsertAsync(cajero);


            return cajero;
        }


        // =========================================================
        // ACTUALIZAR
        // =========================================================

        public async Task
            ActualizarAsync(
                Cajero cajero)
        {
            if (cajero == null)
                throw new ArgumentNullException(
                    nameof(cajero));


            if (string.IsNullOrWhiteSpace(
                cajero.Nombre))
            {
                throw new ArgumentException(
                    "El nombre del cajero es obligatorio.");
            }


            var database =
                await _databaseService
                    .GetConnectionAsync();


            cajero.Nombre =
                cajero.Nombre.Trim();


            var existente =
                await database
                    .Table<Cajero>()
                    .Where(x =>
                        x.Nombre == cajero.Nombre &&
                        x.Id != cajero.Id)
                    .FirstOrDefaultAsync();


            if (existente != null)
            {
                throw new InvalidOperationException(
                    "Ya existe otro cajero con ese nombre.");
            }


            await database
                .UpdateAsync(cajero);
        }


        // =========================================================
        // DESACTIVAR
        // =========================================================

        public async Task
            DesactivarAsync(
                Cajero cajero)
        {
            if (cajero == null)
                return;


            var database =
                await _databaseService
                    .GetConnectionAsync();


            cajero.Activo = false;


            await database
                .UpdateAsync(cajero);
        }


        // =========================================================
        // ACTIVAR
        // =========================================================

        public async Task
            ActivarAsync(
                Cajero cajero)
        {
            if (cajero == null)
                return;


            var database =
                await _databaseService
                    .GetConnectionAsync();


            cajero.Activo = true;


            await database
                .UpdateAsync(cajero);
        }
    }
}