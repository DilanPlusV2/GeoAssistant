using SQLite;
using geocode.models;
using ClosedXML.Excel;
using Microsoft.Maui.Storage;

namespace geocode.services
{
    public class JornadaService
    {
        private readonly DatabaseService _databaseService;

        public JornadaService(
            DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // =========================================================
        // OBTENER JORNADA ABIERTA
        // =========================================================

        public async Task<Jornada?> ObtenerJornadaAbiertaAsync()
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();

            return await database
                .Table<Jornada>()
                .Where(x =>
                    x.Estado == "ABIERTA")
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
        }


        // =========================================================
        // OBTENER TODAS LAS JORNADAS
        // =========================================================

        public async Task<List<Jornada>> ObtenerTodasAsync()
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();

            return await database
                .Table<Jornada>()
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }


        // =========================================================
        // CREAR JORNADA
        // =========================================================

        public async Task<Jornada> CrearJornadaAsync(
            DateTime fechaInicio)
        {
            var jornadaAbierta =
                await ObtenerJornadaAbiertaAsync();

            if (jornadaAbierta != null)
            {
                throw new InvalidOperationException(
                    "Ya existe una jornada abierta."
                );
            }

            var database =
                await _databaseService
                    .GetConnectionAsync();

            var jornada = new Jornada
            {
                FechaInicio =
                    fechaInicio.ToString(
                        "yyyy-MM-dd HH:mm:ss"),

                FechaFin = null,

                Estado = "ABIERTA",

                Observacion = null
            };

            await database.InsertAsync(jornada);

            return jornada;
        }


        // =========================================================
        // CERRAR JORNADA
        // =========================================================

        public async Task CerrarJornadaAsync(
            Jornada jornada,
            string? observacion = null)
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();

            jornada.Estado = "CERRADA";

            jornada.FechaFin =
                DateTime.Now.ToString(
                    "yyyy-MM-dd HH:mm:ss");

            if (!string.IsNullOrWhiteSpace(
                observacion))
            {
                jornada.Observacion =
                    observacion;
            }

            await database.UpdateAsync(jornada);
        }


        // =========================================================
        // OBTENER POR ID
        // =========================================================

        public async Task<Jornada?> ObtenerPorIdAsync(
            int id)
        {
            var database =
                await _databaseService
                    .GetConnectionAsync();

            return await database
                .Table<Jornada>()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // EXPORTAR HISTORIAL DE JORNADA A EXCEL
        // =========================================================

        public async Task<string> ExportarJornadaExcelAsync(
            Jornada jornada)
        {
            if (jornada == null)
                throw new ArgumentNullException(nameof(jornada));

            var database =
                await _databaseService
                    .GetConnectionAsync();

            // =====================================================
            // OBTENER ENTREGAS DE LA JORNADA
            // =====================================================

            var entregas =
                await database
                    .Table<Entrega>()
                    .Where(x =>
                        x.JornadaId == jornada.Id)
                    .OrderBy(x => x.FechaHora)
                    .ToListAsync();

            // =====================================================
            // OBTENER CONFIGURACIONES
            // =====================================================

            var configuraciones =
                await database
                    .Table<ConfiguracionFacturacion>()
                    .ToListAsync();

            // =====================================================
            // OBTENER CAJEROS
            // =====================================================

            var cajeros =
                await database
                    .Table<Cajero>()
                    .ToListAsync();

            // =====================================================
            // OBTENER DOMICILIARIOS
            // =====================================================

            var domiciliarios =
                await database
                    .Table<Domiciliario>()
                    .ToListAsync();

            // =====================================================
            // AGRUPAR ENTREGAS POR CONFIGURACIÓN
            // =====================================================

            var grupos =
                entregas
                    .GroupBy(x => x.ConfiguracionFacturacionId)
                    .Select(grupo =>
                    {
                        var configuracion =
                            configuraciones.FirstOrDefault(
                                x => x.Id == grupo.Key);

                        var cajero =
                            configuracion == null
                                ? null
                                : cajeros.FirstOrDefault(
                                    x =>
                                        x.Id ==
                                        configuracion.CajeroId);

                        return new
                        {
                            Configuracion = configuracion,
                            Cajero = cajero,
                            Entregas = grupo
                                .OrderBy(x => x.FechaHora)
                                .ToList()
                        };
                    })
                    .Where(x =>
                        x.Configuracion != null)
                    .OrderBy(x =>
                        x.Configuracion!.Tipo == "ELECTRONICA"
                            ? 1
                            : 0)
                    .ThenBy(x =>
                        x.Configuracion!.Serie)
                    .ToList();

            // =====================================================
            // CREAR LIBRO
            // =====================================================

            using var workbook =
                new XLWorkbook();

            var worksheet =
                workbook.Worksheets.Add(
                    "Jornada");

            // =====================================================
            // FECHA DE LA JORNADA
            // =====================================================

            worksheet.Cell(1, 1).Value =
                ObtenerFechaJornada(jornada);

            worksheet.Cell(1, 1)
                .Style.Font.Bold = true;

            worksheet.Cell(1, 1)
                .Style.Font.FontSize = 16;

            // =====================================================
            // SI NO HAY ENTREGAS
            // =====================================================

            if (grupos.Count == 0)
            {
                worksheet.Cell(3, 1).Value =
                    "No hay pedidos registrados en esta jornada.";

                worksheet.Columns()
                    .AdjustToContents();

                var rutaVacia =
                    CrearRutaArchivo(jornada);

                workbook.SaveAs(rutaVacia);

                return rutaVacia;
            }

            // =====================================================
            // COLUMNAS
            // =====================================================

            var columna = 1;

            foreach (var grupo in grupos)
            {
                var configuracion =
                    grupo.Configuracion!;

                var esElectronica =
                    configuracion.Tipo
                        .Equals(
                            "ELECTRONICA",
                            StringComparison.OrdinalIgnoreCase);

                // -------------------------------------------------
                // ENCABEZADO
                // -------------------------------------------------

                var celdaEncabezado =
                    worksheet.Cell(3, columna);

                celdaEncabezado.Value =
                    esElectronica
                        ? "Factura electrónica"
                        : "cajero";

                celdaEncabezado.Style.Font.Bold = true;

                celdaEncabezado.Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                // -------------------------------------------------
                // INFORMACIÓN DEL CAJERO
                // -------------------------------------------------

                if (!esElectronica)
                {
                    var nombreCajero =
                        grupo.Cajero?.Nombre
                        ?? "Sin nombre";

                    worksheet.Cell(4, columna)
                        .Value =
                        $"serie: {configuracion.Serie} " +
                        $"nombre: {nombreCajero}";

                    worksheet.Cell(4, columna)
                        .Style.Font.Bold = true;
                }

                // -------------------------------------------------
                // PEDIDOS
                // -------------------------------------------------

                var fila = 5;

                foreach (var entrega in grupo.Entregas)
                {
                    var domiciliario =
                        domiciliarios.FirstOrDefault(
                            x =>
                                x.Id ==
                                entrega.DomiciliarioId);

                    var codigoDomiciliario =
                        domiciliario?.Codigo
                        ?? string.Empty;

                    var texto =
                        string.IsNullOrWhiteSpace(
                            codigoDomiciliario)
                            ? entrega.NumeroFactura
                            : $"{entrega.NumeroFactura} - " +
                              $"{codigoDomiciliario}";

                    worksheet.Cell(fila, columna)
                        .Value = texto;

                    fila++;
                }

                columna++;
            }

            // =====================================================
            // FORMATO
            // =====================================================

            var ultimaColumna =
                Math.Max(1, columna - 1);

            var rangoEncabezado =
                worksheet.Range(
                    3,
                    1,
                    4,
                    ultimaColumna);

            rangoEncabezado.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            rangoEncabezado.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            rangoEncabezado.Style.Alignment.WrapText =
                true;

            var rangoDatos =
                worksheet.Range(
                    5,
                    1,
                    worksheet.LastRowUsed()?.RowNumber() ?? 5,
                    ultimaColumna);

            rangoDatos.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            rangoDatos.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            // =====================================================
            // BORDES
            // =====================================================

            var ultimaFila =
                worksheet.LastRowUsed()?.RowNumber() ?? 4;

            worksheet.Range(
                3,
                1,
                ultimaFila,
                ultimaColumna)
                .Style.Border.OutsideBorder =
                XLBorderStyleValues.Thin;

            worksheet.Range(
                3,
                1,
                ultimaFila,
                ultimaColumna)
                .Style.Border.InsideBorder =
                XLBorderStyleValues.Thin;

            // =====================================================
            // ANCHO DE COLUMNAS
            // =====================================================

            for (var i = 1; i <= ultimaColumna; i++)
            {
                worksheet.Column(i)
                    .AdjustToContents();

                // Evitar columnas exageradamente anchas
                if (worksheet.Column(i).Width > 35)
                {
                    worksheet.Column(i).Width = 35;
                }
            }

            worksheet.Row(1).Height = 25;
            worksheet.Row(3).Height = 25;
            worksheet.Row(4).Height = 30;

            // =====================================================
            // CREAR ARCHIVO
            // =====================================================

            var ruta =
                CrearRutaArchivo(jornada);

            workbook.SaveAs(ruta);

            return ruta;
        }
        // =========================================================
        // FECHA DE LA JORNADA
        // =========================================================

        private string ObtenerFechaJornada(
            Jornada jornada)
        {
            if (DateTime.TryParse(
                jornada.FechaInicio,
                out var fecha))
            {
                return fecha.ToString("dd/MM/yyyy");
            }

            return jornada.FechaInicio;
        }


        // =========================================================
        // RUTA DEL ARCHIVO
        // =========================================================

        private string CrearRutaArchivo(
            Jornada jornada)
        {
            var nombreArchivo =
                $"Jornada_{jornada.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return Path.Combine(
                FileSystem.CacheDirectory,
                nombreArchivo);
        }
    }

}