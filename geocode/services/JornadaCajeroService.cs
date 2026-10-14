using SQLite;
using geocode.models;

namespace geocode.services
{
    public class JornadaCajeroService
    {
        private readonly DatabaseService _databaseService;

        public JornadaCajeroService(
            DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // =========================================================
        // OBTENER ASIGNACIONES DE UNA JORNADA
        // =========================================================

        public async Task<List<JornadaCajero>>
            ObtenerPorJornadaAsync(
                Jornada jornada)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<JornadaCajero>()
                .Where(x =>
                    x.JornadaId == jornada.Id &&
                    x.Activo)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }


        // =========================================================
        // OBTENER UNA ASIGNACIÓN
        // =========================================================

        public async Task<JornadaCajero?>
            ObtenerAsync(
                Jornada jornada,
                int cajeroId,
                int configuracionFacturacionId)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<JornadaCajero>()
                .Where(x =>
                    x.JornadaId == jornada.Id &&
                    x.CajeroId == cajeroId &&
                    x.ConfiguracionFacturacionId ==
                        configuracionFacturacionId &&
                    x.Activo)
                .FirstOrDefaultAsync();
        }


        // =========================================================
        // CREAR
        // =========================================================

        public async Task<JornadaCajero>
            CrearAsync(
                Jornada jornada,
                Cajero cajero,
                ConfiguracionFacturacion configuracion,
                int numeroFacturaInicial)
        {
            if (numeroFacturaInicial < 0)
            {
                throw new ArgumentException(
                    "El número inicial de factura no es válido.");
            }

            var database =
                await _databaseService.GetConnectionAsync();

            var existente =
                await ObtenerAsync(
                    jornada,
                    cajero.Id,
                    configuracion.Id);

            if (existente != null)
            {
                throw new InvalidOperationException(
                    $"El cajero {cajero.Nombre} ya está " +
                    $"asignado a esta jornada.");
            }

            var jornadaCajero =
                new JornadaCajero
                {
                    JornadaId = jornada.Id,

                    CajeroId = cajero.Id,

                    ConfiguracionFacturacionId =
                        configuracion.Id,

                    NumeroFacturaInicial =
                        numeroFacturaInicial,

                    Activo = true
                };

            await database.InsertAsync(
                jornadaCajero);

            return jornadaCajero;
        }


        // =========================================================
        // ACTUALIZAR
        // =========================================================

        public async Task ActualizarAsync(
            JornadaCajero jornadaCajero,
            int nuevoNumero)
        {
            if (nuevoNumero < 0)
            {
                throw new ArgumentException(
                    "El número inicial de factura no es válido.");
            }

            var database =
                await _databaseService.GetConnectionAsync();

            jornadaCajero.NumeroFacturaInicial =
                nuevoNumero;

            await database.UpdateAsync(
                jornadaCajero);
        }


        // =========================================================
        // DESACTIVAR
        // =========================================================

        public async Task DesactivarAsync(
            JornadaCajero jornadaCajero)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            jornadaCajero.Activo = false;

            await database.UpdateAsync(
                jornadaCajero);
        }


        // =========================================================
        // ACTIVAR
        // =========================================================

        public async Task ActivarAsync(
            JornadaCajero jornadaCajero)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            jornadaCajero.Activo = true;

            await database.UpdateAsync(
                jornadaCajero);
        }


        // =========================================================
        // BUSCAR DUEÑO DE FACTURA NORMAL
        // =========================================================
        //
        // Se utiliza cuando el usuario escribe solamente:
        //
        // 26350-D
        //
        // y no proporciona la serie.
        //
        // Busca el cajero cuya numeración inicial sea
        // menor o igual al número de factura.
        //
        // Ejemplo:
        //
        // Cajero 1 -> 26000
        // Cajero 2 -> 26300
        // Cajero 3 -> 26500
        //
        // Factura -> 26350
        //
        // Resultado:
        // Cajero 2
        //
        // =========================================================

        public async Task<JornadaCajero?>
            BuscarFacturaNormalAsync(
                Jornada jornada,
                int numeroFactura)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<JornadaCajero>()
                .Where(x =>
                    x.JornadaId == jornada.Id &&
                    x.Activo &&
                    x.NumeroFacturaInicial <=
                        numeroFactura)
                .OrderByDescending(
                    x => x.NumeroFacturaInicial)
                .FirstOrDefaultAsync();
        }


        // =========================================================
        // BUSCAR DUEÑO DE FACTURA POR CONFIGURACIÓN
        // =========================================================
        //
        // Se utiliza cuando conocemos la configuración de
        // facturación, por ejemplo:
        //
        // Serie: 007
        // Tipo: NORMAL
        //
        // o:
        //
        // Serie: FE
        // Tipo: ELECTRONICA
        //
        // La configuración ya contiene el ID que fue asignado
        // al cajero durante la jornada.
        //
        // =========================================================

        public async Task<JornadaCajero?>
            BuscarPorFacturaAsync(
                Jornada jornada,
                ConfiguracionFacturacion configuracion,
                int numeroFactura)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<JornadaCajero>()
                .Where(x =>
                    x.JornadaId == jornada.Id &&
                    x.Activo &&
                    x.ConfiguracionFacturacionId ==
                        configuracion.Id &&
                    x.NumeroFacturaInicial <=
                        numeroFactura)
                .OrderByDescending(
                    x => x.NumeroFacturaInicial)
                .FirstOrDefaultAsync();
        }
    }
}