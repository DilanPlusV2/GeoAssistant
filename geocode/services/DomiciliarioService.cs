using geocode.models;

namespace geocode.services
{
    public class DomiciliarioService
    {
        private readonly DatabaseService _databaseService;

        public DomiciliarioService(
            DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }


        // =========================================================
        // OBTENER POR CÓDIGO
        // =========================================================

        public async Task<Domiciliario?>
            ObtenerPorCodigoAsync(
                string codigo)
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();

            codigo =
                codigo.Trim()
                      .ToUpperInvariant();

            return await database
                .Table<Domiciliario>()
                .Where(x =>
                    x.Codigo == codigo &&
                    x.Activo)
                .FirstOrDefaultAsync();
        }


        // =========================================================
        // OBTENER TODOS LOS ACTIVOS
        // =========================================================

        public async Task<List<Domiciliario>>
            ObtenerTodosAsync()
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();

            return await database
                .Table<Domiciliario>()
                .Where(x =>
                    x.Activo)
                .OrderBy(x =>
                    x.Codigo)
                .ToListAsync();
        }


        // =========================================================
        // OBTENER ACTIVOS
        // =========================================================

        public async Task<List<Domiciliario>>
            ObtenerActivosAsync()
        {
            return await ObtenerTodosAsync();
        }


        // =========================================================
        // VERIFICAR SI EXISTE CÓDIGO
        // =========================================================

        public async Task<bool>
            ExisteCodigoAsync(
                string codigo)
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();

            codigo =
                codigo.Trim()
                      .ToUpperInvariant();

            var existente =
                await database
                    .Table<Domiciliario>()
                    .Where(x =>
                        x.Codigo == codigo &&
                        x.Activo)
                    .FirstOrDefaultAsync();

            return existente != null;
        }


        // =========================================================
        // CREAR DOMICILIARIO
        // =========================================================

        public async Task<Domiciliario>
            CrearAsync(
                string codigo,
                string? nombre)
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();

            codigo =
                codigo.Trim()
                      .ToUpperInvariant();

            nombre =
                string.IsNullOrWhiteSpace(nombre)
                    ? null
                    : nombre.Trim();

            // -----------------------------------------------------
            // EVITAR CÓDIGOS REPETIDOS
            // -----------------------------------------------------

            var existente =
                await database
                    .Table<Domiciliario>()
                    .Where(x =>
                        x.Codigo == codigo)
                    .FirstOrDefaultAsync();

            if (existente != null)
            {
                // Si existía pero estaba inactivo,
                // lo podemos reactivar.
                if (!existente.Activo)
                {
                    existente.Activo = true;
                    existente.Nombre = nombre;

                    await database.UpdateAsync(
                        existente);

                    return existente;
                }

                throw new InvalidOperationException(
                    $"Ya existe un domiciliario con el código {codigo}.");
            }


            // -----------------------------------------------------
            // CREAR
            // -----------------------------------------------------

            var domiciliario =
                new Domiciliario
                {
                    Codigo = codigo,
                    Nombre = nombre,
                    Activo = true
                };

            await database.InsertAsync(
                domiciliario);

            return domiciliario;
        }


        // =========================================================
        // ACTUALIZAR
        // =========================================================

        public async Task<Domiciliario>
            ActualizarAsync(
                Domiciliario domiciliario,
                string codigo,
                string? nombre)
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();

            codigo =
                codigo.Trim()
                      .ToUpperInvariant();

            nombre =
                string.IsNullOrWhiteSpace(nombre)
                    ? null
                    : nombre.Trim();


            // -----------------------------------------------------
            // VERIFICAR QUE EL CÓDIGO NO PERTENEZCA A OTRO
            // -----------------------------------------------------

            var existente =
                await database
                    .Table<Domiciliario>()
                    .Where(x =>
                        x.Codigo == codigo &&
                        x.Id != domiciliario.Id)
                    .FirstOrDefaultAsync();

            if (existente != null)
            {
                throw new InvalidOperationException(
                    $"Ya existe otro domiciliario con el código {codigo}.");
            }


            domiciliario.Codigo = codigo;
            domiciliario.Nombre = nombre;

            await database.UpdateAsync(
                domiciliario);

            return domiciliario;
        }


        // =========================================================
        // DESACTIVAR
        // =========================================================

        public async Task
            DesactivarAsync(
                Domiciliario domiciliario)
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();

            domiciliario.Activo = false;

            await database.UpdateAsync(
                domiciliario);
        }


        // =========================================================
        // ACTIVAR
        // =========================================================

        public async Task
            ActivarAsync(
                Domiciliario domiciliario)
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();

            domiciliario.Activo = true;

            await database.UpdateAsync(
                domiciliario);
        }
    }
}