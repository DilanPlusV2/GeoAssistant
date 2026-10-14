using geocode.models;

namespace geocode.services
{
    public class EntregaService
    {
        private readonly DatabaseService _databaseService;

        public EntregaService(
            DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }


        // =========================================================
        // REGISTRAR ENTREGA
        // =========================================================

        public async Task<Entrega> RegistrarAsync(
            Jornada jornada,
            FacturaAsignada factura,
            Domiciliario domiciliario)
        {
            var database =
                await _databaseService.GetConnectionAsync();


            // =====================================================
            // CREAR SIEMPRE UNA NUEVA ENTREGA
            // =====================================================

            var entrega = new Entrega
            {
                JornadaId =
                    jornada.Id,

                ConfiguracionFacturacionId =
                    factura.ConfiguracionFacturacionId,

                DomiciliarioId =
                    domiciliario.Id,

                NumeroFactura =
                    factura.NumeroFactura,

                TipoFactura =
                    "NORMAL",

                FechaHora =
                    DateTime.Now
            };


            await database.InsertAsync(
                entrega);


            // =====================================================
            // MARCAR FACTURA COMO UTILIZADA
            // =====================================================

            factura.Utilizada = true;

            await database.UpdateAsync(
                factura);


            return entrega;
        }


        // =========================================================
        // REGISTRAR ENTREGA CON DATOS ADICIONALES
        // =========================================================

        public async Task<Entrega> RegistrarAsync(
            Jornada jornada,
            FacturaAsignada factura,
            Domiciliario domiciliario,
            string tipoFactura,
            string? barrio = null,
            string? observacion = null)
        {
            var database =
                await _databaseService.GetConnectionAsync();


            // =====================================================
            // CREAR SIEMPRE UNA NUEVA ENTREGA
            // =====================================================

            var entrega = new Entrega
            {
                JornadaId =
                    jornada.Id,

                ConfiguracionFacturacionId =
                    factura.ConfiguracionFacturacionId,

                DomiciliarioId =
                    domiciliario.Id,

                NumeroFactura =
                    factura.NumeroFactura,

                TipoFactura =
                    tipoFactura,

                Barrio =
                    barrio,

                FechaHora =
                    DateTime.Now,

                Observacion =
                    observacion
            };


            await database.InsertAsync(
                entrega);


            // =====================================================
            // MARCAR FACTURA COMO UTILIZADA
            // =====================================================

            factura.Utilizada = true;

            await database.UpdateAsync(
                factura);


            return entrega;
        }


        // =========================================================
        // OBTENER TODAS LAS ENTREGAS DE UNA JORNADA
        // =========================================================

        public async Task<List<Entrega>>
            ObtenerEntregasAsync(
                Jornada jornada)
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();


            return await database
                .Table<Entrega>()
                .Where(x =>
                    x.JornadaId == jornada.Id)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }


        // =========================================================
        // BUSCAR TODAS LAS ENTREGAS DE UNA FACTURA
        // =========================================================
        //
        // IMPORTANTE:
        //
        // Una factura puede tener varias entregas.
        //
        // Ejemplo:
        //
        // 24984 - D
        // 24984 - V
        // 24984 - D
        //
        // Por eso este método devuelve LISTA.
        //
        // =========================================================

        public async Task<List<Entrega>>
            BuscarEntregasAsync(
                Jornada jornada,
                int configuracionFacturacionId,
                string numeroFactura)
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();


            return await database
                .Table<Entrega>()
                .Where(x =>
                    x.JornadaId == jornada.Id &&
                    x.ConfiguracionFacturacionId ==
                        configuracionFacturacionId &&
                    x.NumeroFactura ==
                        numeroFactura)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }


        // =========================================================
        // BUSCAR LA ÚLTIMA ENTREGA DE UNA FACTURA
        // =========================================================
        //
        // Este método es útil si en alguna parte del programa
        // solamente necesitas la última entrega registrada.
        //
        // NO debe utilizarse para la consulta general.
        //
        // =========================================================

        public async Task<Entrega?>
            BuscarUltimaEntregaAsync(
                Jornada jornada,
                int configuracionFacturacionId,
                string numeroFactura)
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();


            return await database
                .Table<Entrega>()
                .Where(x =>
                    x.JornadaId == jornada.Id &&
                    x.ConfiguracionFacturacionId ==
                        configuracionFacturacionId &&
                    x.NumeroFactura ==
                        numeroFactura)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
        }
    }
}