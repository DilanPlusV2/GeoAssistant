using geocode.models;

namespace geocode.services
{
    public class ConfiguracionFacturacionService
    {
        private readonly DatabaseService _databaseService;

        public ConfiguracionFacturacionService(
            DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // =========================================================
        // OBTENER TODAS
        // =========================================================

        public async Task<List<ConfiguracionFacturacion>>
            ObtenerTodasAsync()
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<ConfiguracionFacturacion>()
                .OrderBy(x => x.Serie)
                .ToListAsync();
        }

        // =========================================================
        // OBTENER ACTIVAS
        // =========================================================

        public async Task<List<ConfiguracionFacturacion>>
            ObtenerActivasAsync()
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<ConfiguracionFacturacion>()
                .Where(x => x.Activa)
                .OrderBy(x => x.Serie)
                .ToListAsync();
        }

        // =========================================================
        // BUSCAR POR SERIE
        // =========================================================

        public async Task<ConfiguracionFacturacion?>
            BuscarPorSerieAsync(
                string serie)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            serie = NormalizarSerie(serie);

            return await database
                .Table<ConfiguracionFacturacion>()
                .Where(x =>
                    x.Serie == serie &&
                    x.Activa)
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // BUSCAR POR SERIE Y TIPO
        // =========================================================

        public async Task<ConfiguracionFacturacion?>
            BuscarAsync(
                string serie,
                string tipo)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            serie = NormalizarSerie(serie);

            tipo = tipo
                .Trim()
                .ToUpperInvariant();

            return await database
                .Table<ConfiguracionFacturacion>()
                .Where(x =>
                    x.Serie == serie &&
                    x.Tipo == tipo &&
                    x.Activa)
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // BUSCAR POR ID
        // =========================================================

        public async Task<ConfiguracionFacturacion?>
            ObtenerPorIdAsync(
                int id)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<ConfiguracionFacturacion>()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // CREAR
        // =========================================================

        public async Task<ConfiguracionFacturacion>
            CrearAsync(
                int cajeroId,
                string serie,
                string tipo = "NORMAL")
        {
            var database =
                await _databaseService.GetConnectionAsync();

            serie = NormalizarSerie(serie);

            tipo = NormalizarTipo(tipo);

            if (string.IsNullOrWhiteSpace(serie))
                throw new Exception(
                    "La serie es obligatoria.");

            // -----------------------------------------------------
            // EVITAR DUPLICADOS
            // -----------------------------------------------------

            var existente =
                await database
                    .Table<ConfiguracionFacturacion>()
                    .Where(x =>
                        x.Serie == serie &&
                        x.Tipo == tipo)
                    .FirstOrDefaultAsync();

            if (existente != null)
            {
                throw new Exception(
                    $"Ya existe una configuración " +
                    $"con serie {serie} y tipo {tipo}.");
            }

            var configuracion =
                new ConfiguracionFacturacion
                {
                    CajeroId = cajeroId,
                    Serie = serie,
                    Tipo = tipo,
                    Activa = true
                };

            await database.InsertAsync(
                configuracion);

            return configuracion;
        }

        // =========================================================
        // ACTUALIZAR
        // =========================================================

        public async Task ActualizarAsync(
            ConfiguracionFacturacion configuracion)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            configuracion.Serie =
                NormalizarSerie(
                    configuracion.Serie);

            configuracion.Tipo =
                NormalizarTipo(
                    configuracion.Tipo);

            if (string.IsNullOrWhiteSpace(
                configuracion.Serie))
            {
                throw new Exception(
                    "La serie es obligatoria.");
            }

            // -----------------------------------------------------
            // BUSCAR DUPLICADO EXCLUYENDO EL ACTUAL
            // -----------------------------------------------------

            var existente =
                await database
                    .Table<ConfiguracionFacturacion>()
                    .Where(x =>
                        x.Id != configuracion.Id &&
                        x.Serie ==
                            configuracion.Serie &&
                        x.Tipo ==
                            configuracion.Tipo)
                    .FirstOrDefaultAsync();

            if (existente != null)
            {
                throw new Exception(
                    $"Ya existe otra configuración " +
                    $"con serie {configuracion.Serie} " +
                    $"y tipo {configuracion.Tipo}.");
            }

            await database.UpdateAsync(
                configuracion);
        }

        // =========================================================
        // ACTIVAR
        // =========================================================

        public async Task ActivarAsync(
            ConfiguracionFacturacion configuracion)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            configuracion.Activa = true;

            await database.UpdateAsync(
                configuracion);
        }

        // =========================================================
        // DESACTIVAR
        // =========================================================

        public async Task DesactivarAsync(
            ConfiguracionFacturacion configuracion)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            configuracion.Activa = false;

            await database.UpdateAsync(
                configuracion);
        }

        // =========================================================
        // NORMALIZAR SERIE
        // =========================================================

        private string NormalizarSerie(
            string serie)
        {
            if (string.IsNullOrWhiteSpace(serie))
                return string.Empty;

            serie = serie.Trim();

            // 7  -> 007
            // 8  -> 008
            // 06 -> 006

            if (int.TryParse(
                serie,
                out int numero))
            {
                return numero.ToString("D3");
            }

            return serie.ToUpperInvariant();
        }

        // =========================================================
        // NORMALIZAR TIPO
        // =========================================================

        private string NormalizarTipo(
            string tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
                return "NORMAL";

            return tipo
                .Trim()
                .ToUpperInvariant();
        }
    }
}