using geocode.models;

namespace geocode.services
{
    public class ConsultaEntregaService
    {
        private readonly DatabaseService _databaseService;

        public ConsultaEntregaService(
            DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }


        // =========================================================
        // OBTENER ENTREGAS DE LA JORNADA ACTUAL
        // =========================================================

        public async Task<List<ConsultaEntregaItem>>
    ObtenerEntregasJornadaAsync(
        Jornada jornada)
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();


            // =====================================================
            // 1. OBTENER TODAS LAS ENTREGAS
            // =====================================================

            var entregas =
                await database
                    .Table<Entrega>()
                    .Where(x =>
                        x.JornadaId == jornada.Id)
                    .OrderBy(x => x.Id)
                    .ToListAsync();


            var resultado =
                new List<ConsultaEntregaItem>();


            // =====================================================
            // 2. PROCESAR CADA ENTREGA
            // =====================================================

            foreach (var entrega in entregas)
            {
                // =================================================
                // BUSCAR DOMICILIARIO
                // =================================================

                var domiciliario =
                    await database
                        .Table<Domiciliario>()
                        .Where(x =>
                            x.Id ==
                            entrega.DomiciliarioId)
                        .FirstOrDefaultAsync();


                // =================================================
                // BUSCAR CONFIGURACIÓN
                // =================================================

                var configuracion =
                    await database
                        .Table<ConfiguracionFacturacion>()
                        .Where(x =>
                            x.Id ==
                            entrega.ConfiguracionFacturacionId)
                        .FirstOrDefaultAsync();


                if (configuracion == null)
                    continue;


                // =================================================
                // BUSCAR CAJERO
                // =================================================

                var cajero =
                    await database
                        .Table<Cajero>()
                        .Where(x =>
                            x.Id ==
                            configuracion.CajeroId)
                        .FirstOrDefaultAsync();


                // =================================================
                // CREAR RESULTADO
                // =================================================
                //
                // IMPORTANTE:
                //
                // NO buscamos FacturaAsignada.
                //
                // Una entrega ya es un registro independiente.
                // Por lo tanto, si existen:
                //
                // 24984 - D
                // 24984 - V
                //
                // ambas deben aparecer.
                //
                // =================================================

                resultado.Add(
                    new ConsultaEntregaItem
                    {
                        EntregaId =
                            entrega.Id,

                        NumeroFactura =
                            entrega.NumeroFactura,

                        Serie =
                            configuracion.Serie
                            ?? string.Empty,

                        TipoFactura =
                            configuracion.Tipo
                            ?? string.Empty,

                        Cajero =
                            cajero?.Nombre
                            ?? "Sin cajero",

                        Domiciliario =
                            domiciliario?.Codigo
                            ?? string.Empty,

                        Utilizada = true
                    });
            }


            // =====================================================
            // 3. MANTENER EL ORDEN REAL DE REGISTRO
            // =====================================================
            //
            // NO agrupamos por número de factura.
            // NO eliminamos duplicados.
            //
            // El Id de Entrega representa el orden en que
            // fueron registradas.
            //
            // =====================================================

            return resultado
                .OrderBy(x => x.EntregaId)
                .ToList();
        }
    }
}