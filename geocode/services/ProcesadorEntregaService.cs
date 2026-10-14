using geocode.models;

namespace geocode.services
{
    public class ProcesadorEntregaService
    {
        private readonly JornadaService _jornadaService;
        private readonly FacturaService _facturaService;
        private readonly CajeroService _cajeroService;
        private readonly DomiciliarioService _domiciliarioService;
        private readonly ConfiguracionFacturacionService _configuracionService;
        private readonly EntregaParserService _parserService;
        private readonly EntregaService _entregaService;
        private readonly JornadaCajeroService _jornadaCajeroService;

        public ProcesadorEntregaService(
            JornadaService jornadaService,
            FacturaService facturaService,
            CajeroService cajeroService,
            DomiciliarioService domiciliarioService,
            ConfiguracionFacturacionService configuracionService,
            EntregaParserService parserService,
            EntregaService entregaService,
            JornadaCajeroService jornadaCajeroService)
        {
            _jornadaService = jornadaService;
            _facturaService = facturaService;
            _cajeroService = cajeroService;
            _domiciliarioService = domiciliarioService;
            _configuracionService = configuracionService;
            _parserService = parserService;
            _entregaService = entregaService;
            _jornadaCajeroService = jornadaCajeroService;
        }


        // =========================================================
        // PROCESAR ENTREGA
        // =========================================================

        public async Task<ResultadoEntrega> ProcesarAsync(
            string texto)
        {
            // =====================================================
            // 1. INTERPRETAR TEXTO
            // =====================================================

            var entrada =
                _parserService.Parsear(texto);

            if (!entrada.EsValida)
            {
                return ResultadoEntrega.Error(
                    entrada.Error ??
                    "Entrada no válida."
                );
            }


            // =====================================================
            // 2. OBTENER JORNADA ACTUAL
            // =====================================================

            var jornada =
                await _jornadaService
                    .ObtenerJornadaAbiertaAsync();

            if (jornada == null)
            {
                return ResultadoEntrega.Error(
                    "No hay una jornada abierta. " +
                    "Debes abrir una jornada antes de registrar entregas."
                );
            }


            // =====================================================
            // 3. BUSCAR DOMICILIARIO
            // =====================================================

            if (string.IsNullOrWhiteSpace(
                entrada.CodigoDomiciliario))
            {
                return ResultadoEntrega.Error(
                    "No se indicó el código del domiciliario."
                );
            }

            var domiciliario =
                await _domiciliarioService
                    .ObtenerPorCodigoAsync(
                        entrada.CodigoDomiciliario
                    );

            if (domiciliario == null)
            {
                return ResultadoEntrega.Error(
                    $"No existe el domiciliario " +
                    $"{entrada.CodigoDomiciliario}."
                );
            }


            // =====================================================
            // 4. VALIDAR NÚMERO DE FACTURA
            // =====================================================

            if (!int.TryParse(
                entrada.NumeroFactura,
                out int numeroFactura))
            {
                return ResultadoEntrega.Error(
                    "El número de factura no es válido."
                );
            }


            // =====================================================
            // 5. DETERMINAR CONFIGURACIÓN
            // =====================================================

            ConfiguracionFacturacion? configuracion = null;


            // =====================================================
            // FACTURA ELECTRÓNICA
            // =====================================================

            if (entrada.Tipo == "ELECTRONICA")
            {
                if (string.IsNullOrWhiteSpace(
                    entrada.Serie))
                {
                    return ResultadoEntrega.Error(
                        "La factura electrónica " +
                        "no contiene una serie válida."
                    );
                }

                configuracion =
                    await _configuracionService
                        .BuscarAsync(
                            entrada.Serie,
                            "ELECTRONICA"
                        );

                if (configuracion == null)
                {
                    return ResultadoEntrega.Error(
                        $"No existe una configuración activa " +
                        $"para la serie {entrada.Serie}."
                    );
                }
            }


            // =====================================================
            // FACTURA NORMAL CON SERIE
            // =====================================================

            else if (!string.IsNullOrWhiteSpace(
                entrada.Serie))
            {
                configuracion =
                    await _configuracionService
                        .BuscarAsync(
                            entrada.Serie,
                            "NORMAL"
                        );

                if (configuracion == null)
                {
                    return ResultadoEntrega.Error(
                        $"No existe una configuración activa " +
                        $"para la serie {entrada.Serie}."
                    );
                }
            }


            // =====================================================
            // FACTURA NORMAL SIN SERIE
            // =====================================================

            else
            {
                var asignacionNormal =
                    await _jornadaCajeroService
                        .BuscarFacturaNormalAsync(
                            jornada,
                            numeroFactura
                        );

                if (asignacionNormal == null)
                {
                    return ResultadoEntrega.Error(
                        $"La factura {entrada.NumeroFactura} " +
                        $"no pertenece a ningún cajero " +
                        $"de la jornada actual."
                    );
                }

                configuracion =
                    await _configuracionService
                        .ObtenerPorIdAsync(
                            asignacionNormal
                                .ConfiguracionFacturacionId
                        );

                if (configuracion == null)
                {
                    return ResultadoEntrega.Error(
                        "No se pudo determinar la configuración " +
                        "de facturación."
                    );
                }
            }


            // =====================================================
            // 6. VERIFICAR CONFIGURACIÓN
            // =====================================================

            if (configuracion == null)
            {
                return ResultadoEntrega.Error(
                    "No se pudo determinar la configuración " +
                    "de facturación."
                );
            }


            // =====================================================
            // 7. BUSCAR ASIGNACIÓN DEL CAJERO
            // =====================================================

            JornadaCajero? asignacion;

            if (entrada.Tipo == "ELECTRONICA")
            {
                asignacion =
                    await _jornadaCajeroService
                        .BuscarPorFacturaAsync(
                            jornada,
                            configuracion,
                            numeroFactura
                        );
            }
            else if (!string.IsNullOrWhiteSpace(
                entrada.Serie))
            {
                asignacion =
                    await _jornadaCajeroService
                        .BuscarPorFacturaAsync(
                            jornada,
                            configuracion,
                            numeroFactura
                        );
            }
            else
            {
                asignacion =
                    await _jornadaCajeroService
                        .BuscarFacturaNormalAsync(
                            jornada,
                            numeroFactura
                        );
            }


            // =====================================================
            // 8. VERIFICAR ASIGNACIÓN
            // =====================================================

            if (asignacion == null)
            {
                return ResultadoEntrega.Error(
                    $"La factura {entrada.NumeroFactura} " +
                    $"no pertenece a ningún cajero " +
                    $"de la jornada actual."
                );
            }


            // =====================================================
            // 9. BUSCAR FACTURA YA REGISTRADA
            // =====================================================

            var factura =
                await _facturaService
                    .BuscarFacturaAsync(
                        jornada,
                        configuracion,
                        entrada.NumeroFactura
                    );


            // =====================================================
            // 10. SI NO EXISTE, CREARLA
            // =====================================================

            if (factura == null)
            {
                factura =
                    await _facturaService
                        .RegistrarFacturaAsync(
                            jornada,
                            configuracion,
                            entrada.NumeroFactura
                        );
            }


            // =====================================================
            // 11. VERIFICAR FACTURA
            // =====================================================

            if (factura == null)
            {
                return ResultadoEntrega.Error(
                    $"No se pudo registrar la factura " +
                    $"{entrada.NumeroFactura}."
                );
            }


            // =====================================================
            // 12. VERIFICAR SI YA FUE UTILIZADA
            // =====================================================

            //if (factura.Utilizada)
            //{
                //return ResultadoEntrega.Error(
                    //$"La factura " +
                   // $"{entrada.NumeroFactura} " +
                   // $"ya fue utilizada."
                //);
            //}


            // =====================================================
            // 13. OBTENER CAJERO
            // =====================================================

            var cajero =
                await _cajeroService
                    .ObtenerPorIdAsync(
                        configuracion.CajeroId
                    );

            if (cajero == null)
            {
                return ResultadoEntrega.Error(
                    "No se pudo determinar " +
                    "el cajero de la factura."
                );
            }


            // =====================================================
            // 14. REGISTRAR ENTREGA
            // =====================================================

            var entrega =
                await _entregaService
                    .RegistrarAsync(
                        jornada,
                        factura,
                        domiciliario
                    );


            // =====================================================
            // 15. CONSTRUIR MENSAJE
            // =====================================================

            string mensaje;

            if (configuracion.Tipo == "ELECTRONICA")
            {
                mensaje =
                    "Entrega registrada\n\n" +
                    $"Factura electrónica: " +
                    $"{configuracion.Serie}-" +
                    $"{factura.NumeroFactura}\n" +
                    $"Cajero: {cajero.Nombre}\n" +
                    $"Domiciliario: " +
                    $"{domiciliario.Codigo}";
            }
            else
            {
                mensaje =
                    "Entrega registrada\n\n" +
                    $"Factura: " +
                    $"{factura.NumeroFactura}\n" +
                    $"Cajero: {cajero.Nombre}\n" +
                    $"Domiciliario: " +
                    $"{domiciliario.Codigo}";
            }


            // =====================================================
            // 16. RESULTADO COMPLETO
            // =====================================================

            return ResultadoEntrega.Exito(
                mensaje,
                entrega,
                factura,
                cajero,
                domiciliario,
                jornada,
                configuracion
            );
        }
    }
}