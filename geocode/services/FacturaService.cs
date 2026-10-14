using geocode.models;

namespace geocode.services
{
    public class FacturaService
    {
        private readonly DatabaseService _databaseService;

        public FacturaService(
            DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // =========================================================
        // REGISTRAR FACTURA
        // =========================================================

        public async Task<FacturaAsignada>
    RegistrarFacturaAsync(
        Jornada jornada,
        ConfiguracionFacturacion configuracion,
        string numeroFactura)
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();

            var factura = new FacturaAsignada
            {
                JornadaId = jornada.Id,

                ConfiguracionFacturacionId =
                    configuracion.Id,

                NumeroFactura =
                    numeroFactura.Trim(),

                Utilizada = false
            };

            await database.InsertAsync(factura);

            return factura;
        }

        // =========================================================
        // BUSCAR FACTURA
        // =========================================================

        public async Task<FacturaAsignada?>
            BuscarFacturaAsync(
                Jornada jornada,
                ConfiguracionFacturacion configuracion,
                string numeroFactura)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<FacturaAsignada>()
                .Where(x =>
                    x.JornadaId == jornada.Id &&
                    x.ConfiguracionFacturacionId ==
                        configuracion.Id &&
                    x.NumeroFactura == numeroFactura)
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // BUSCAR FACTURA DISPONIBLE
        // =========================================================

        public async Task<FacturaAsignada?>
            BuscarFacturaDisponibleAsync(
                Jornada jornada,
                ConfiguracionFacturacion configuracion,
                string numeroFactura)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<FacturaAsignada>()
                .Where(x =>
                    x.JornadaId == jornada.Id &&
                    x.ConfiguracionFacturacionId ==
                        configuracion.Id &&
                    x.NumeroFactura == numeroFactura &&
                    !x.Utilizada)
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // OBTENER FACTURAS DE UNA JORNADA
        // =========================================================

        public async Task<List<FacturaAsignada>>
            ObtenerFacturasAsync(
                Jornada jornada)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<FacturaAsignada>()
                .Where(x =>
                    x.JornadaId == jornada.Id)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        // =========================================================
        // OBTENER FACTURAS DISPONIBLES
        // =========================================================

        public async Task<List<FacturaAsignada>>
            ObtenerFacturasDisponiblesAsync(
                Jornada jornada)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<FacturaAsignada>()
                .Where(x =>
                    x.JornadaId == jornada.Id &&
                    !x.Utilizada)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        // =========================================================
        // OBTENER FACTURAS UTILIZADAS
        // =========================================================

        public async Task<List<FacturaAsignada>>
            ObtenerFacturasUtilizadasAsync(
                Jornada jornada)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<FacturaAsignada>()
                .Where(x =>
                    x.JornadaId == jornada.Id &&
                    x.Utilizada)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        // =========================================================
        // MARCAR COMO UTILIZADA
        // =========================================================

        public async Task MarcarUtilizadaAsync(
            FacturaAsignada factura)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            factura.Utilizada = true;

            await database.UpdateAsync(factura);
        }

        // =========================================================
        // MARCAR COMO DISPONIBLE
        // =========================================================

        public async Task MarcarDisponibleAsync(
            FacturaAsignada factura)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            factura.Utilizada = false;

            await database.UpdateAsync(factura);
        }

        public async Task<FacturaAsignada?>
    BuscarFacturaPorNumeroAsync(
        Jornada jornada,
        string numeroFactura)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<FacturaAsignada>()
                .Where(x =>
                    x.JornadaId == jornada.Id &&
                    x.NumeroFactura == numeroFactura &&
                    x.Utilizada == false)
                .FirstOrDefaultAsync();
        }
    }
}