using System.Collections.ObjectModel;
using System.Windows.Input;
using geocode.models;
using geocode.services;

namespace geocode.viewmodels
{
    public class JornadaViewModel : BindableObject
    {
        // =========================================================
        // SERVICIOS
        // =========================================================

        private readonly JornadaService _jornadaService;

        private readonly CajeroService _cajeroService;

        private readonly ConfiguracionFacturacionService
            _configuracionFacturacionService;

        private readonly JornadaCajeroService
            _jornadaCajeroService;


        // =========================================================
        // COLECCIONES
        // =========================================================

        public ObservableCollection<Jornada> Jornadas { get; }
            = new();

        public ObservableCollection<Cajero> Cajeros { get; }
            = new();

        public ObservableCollection<JornadaCajeroItem>
            CajerosDisponibles
        { get; }
            = new();

        public ObservableCollection<JornadaCajeroItem>
            CajerosJornada
        { get; }
            = new();
        public ObservableCollection<ConfiguracionFacturacion>
    ConfiguracionesFacturacion
        { get; }
    = new();

        // =========================================================
        // CAMPOS
        // =========================================================

        private Jornada? _jornadaActual;

        private bool _procesando;

        private string _mensaje = string.Empty;

        private bool _mostrarGestionConfiguraciones;

        private bool _mostrarFormularioConfiguracion;

        private string _serieConfiguracion = string.Empty;

        private string _tipoConfiguracion = "NORMAL";

        private Cajero? _cajeroConfiguracion;

        private ConfiguracionFacturacion?
            _configuracionEditando;
        // =========================================================
        // CONFIGURACIÓN NUEVA JORNADA
        // =========================================================

        private bool _mostrarConfiguracionNuevaJornada;

        public bool MostrarConfiguracionNuevaJornada
        {
            get => _mostrarConfiguracionNuevaJornada;

            private set
            {
                if (_mostrarConfiguracionNuevaJornada == value)
                    return;

                _mostrarConfiguracionNuevaJornada = value;

                OnPropertyChanged();
            }
        }


        // =========================================================
        // GESTIÓN DE CAJEROS
        // =========================================================

        private bool _mostrarGestionCajeros;

        private bool _mostrarFormularioCajero;

        private string _nombreCajero = string.Empty;

        private Cajero? _cajeroEditando;


        public bool MostrarGestionCajeros
        {
            get => _mostrarGestionCajeros;

            set
            {
                if (_mostrarGestionCajeros == value)
                    return;

                _mostrarGestionCajeros = value;

                OnPropertyChanged();
            }
        }


        public bool MostrarFormularioCajero
        {
            get => _mostrarFormularioCajero;

            set
            {
                if (_mostrarFormularioCajero == value)
                    return;

                _mostrarFormularioCajero = value;

                OnPropertyChanged();
            }
        }


        public string NombreCajero
        {
            get => _nombreCajero;

            set
            {
                if (_nombreCajero == value)
                    return;

                _nombreCajero = value;

                OnPropertyChanged();
            }
        }


        public Cajero? CajeroEditando
        {
            get => _cajeroEditando;

            set
            {
                if (_cajeroEditando == value)
                    return;

                _cajeroEditando = value;

                OnPropertyChanged();

                OnPropertyChanged(
                    nameof(TextoGuardarCajero));
            }
        }


        public string TextoGuardarCajero =>
            CajeroEditando == null
                ? "AGREGAR CAJERO"
                : "GUARDAR CAMBIOS";


        public bool MostrarGestionConfiguraciones
        {
            get => _mostrarGestionConfiguraciones;

            set
            {
                if (_mostrarGestionConfiguraciones == value)
                    return;

                _mostrarGestionConfiguraciones = value;

                OnPropertyChanged();
            }
        }


        public bool MostrarFormularioConfiguracion
        {
            get => _mostrarFormularioConfiguracion;

            set
            {
                if (_mostrarFormularioConfiguracion == value)
                    return;

                _mostrarFormularioConfiguracion = value;

                OnPropertyChanged();
            }
        }


        public string SerieConfiguracion
        {
            get => _serieConfiguracion;

            set
            {
                if (_serieConfiguracion == value)
                    return;

                _serieConfiguracion = value;

                OnPropertyChanged();
            }
        }


        public string TipoConfiguracion
        {
            get => _tipoConfiguracion;

            set
            {
                if (_tipoConfiguracion == value)
                    return;

                _tipoConfiguracion = value;

                OnPropertyChanged();
            }
        }


        public Cajero? CajeroConfiguracion
        {
            get => _cajeroConfiguracion;

            set
            {
                if (_cajeroConfiguracion == value)
                    return;

                _cajeroConfiguracion = value;

                OnPropertyChanged();
            }
        }


        public ConfiguracionFacturacion?
            ConfiguracionEditando
        {
            get => _configuracionEditando;

            set
            {
                if (_configuracionEditando == value)
                    return;

                _configuracionEditando = value;

                OnPropertyChanged();

                OnPropertyChanged(
                    nameof(TextoGuardarConfiguracion));
            }
        }


        public string TextoGuardarConfiguracion =>
            ConfiguracionEditando == null
                ? "AGREGAR CONFIGURACIÓN"
                : "GUARDAR CAMBIOS";

        // =========================================================
        // JORNADA ACTUAL
        // =========================================================

        public Jornada? JornadaActual
        {
            get => _jornadaActual;

            private set
            {
                if (_jornadaActual == value)
                    return;

                _jornadaActual = value;

                OnPropertyChanged();

                OnPropertyChanged(
                    nameof(HayJornadaAbierta));

                OnPropertyChanged(
                    nameof(TextoEstado));

                OnPropertyChanged(
                    nameof(TextoFechaInicio));

                OnPropertyChanged(
                    nameof(TextoFechaFin));
            }
        }


        public bool HayJornadaAbierta =>
            JornadaActual != null &&
            JornadaActual.Estado == "ABIERTA";


        // =========================================================
        // ESTADO
        // =========================================================

        public bool Procesando
        {
            get => _procesando;

            private set
            {
                if (_procesando == value)
                    return;

                _procesando = value;

                OnPropertyChanged();
            }
        }


        public string Mensaje
        {
            get => _mensaje;

            private set
            {
                if (_mensaje == value)
                    return;

                _mensaje = value;

                OnPropertyChanged();
            }
        }


        // =========================================================
        // TEXTOS JORNADA
        // =========================================================

        public string TextoEstado
        {
            get
            {
                if (JornadaActual == null)
                    return "No hay jornada abierta";

                return JornadaActual.Estado == "ABIERTA"
                    ? "Jornada abierta"
                    : "Jornada cerrada";
            }
        }


        public string TextoFechaInicio =>
            JornadaActual?.FechaInicio
            ?? "-";


        public string TextoFechaFin =>
            JornadaActual?.FechaFin
            ?? "-";


        // =========================================================
        // COMANDOS JORNADA
        // =========================================================

        public ICommand AbrirJornadaCommand { get; }

        public ICommand CerrarJornadaCommand { get; }

        public ICommand RecargarCommand { get; }

        public ICommand IniciarJornadaCommand { get; }

        public ICommand ExportarJornadaCommand { get; }


        // =========================================================
        // COMANDOS CAJEROS
        // =========================================================

        public ICommand GestionarCajerosCommand { get; }

        public ICommand NuevoCajeroCommand { get; }

        public ICommand GuardarCajeroCommand { get; }

        public ICommand CancelarCajeroCommand { get; }

        public ICommand EditarCajeroCommand { get; }

        public ICommand DesactivarCajeroCommand { get; }

        public ICommand ActivarCajeroCommand { get; }

        public ICommand GestionarConfiguracionesCommand { get; }

        public ICommand NuevaConfiguracionCommand { get; }

        public ICommand GuardarConfiguracionCommand { get; }

        public ICommand CancelarConfiguracionCommand { get; }

        public ICommand EditarConfiguracionCommand { get; }

        public ICommand DesactivarConfiguracionCommand { get; }

        public ICommand ActivarConfiguracionCommand { get; }


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public JornadaViewModel(
            JornadaService jornadaService,
            CajeroService cajeroService,
            ConfiguracionFacturacionService
                configuracionFacturacionService,
            JornadaCajeroService jornadaCajeroService)
        {
            _jornadaService =
                jornadaService;

            _cajeroService =
                cajeroService;

            _configuracionFacturacionService =
                configuracionFacturacionService;

            _jornadaCajeroService =
                jornadaCajeroService;


            // =====================================================
            // JORNADA
            // =====================================================

            AbrirJornadaCommand =
                new Command(
                    async () =>
                        await AbrirJornadaAsync());


            CerrarJornadaCommand =
                new Command(
                    async () =>
                        await CerrarJornadaAsync());


            RecargarCommand =
                new Command(
                    async () =>
                        await InicializarAsync());


            IniciarJornadaCommand =
                new Command(
                    async () =>
                        await IniciarJornadaConfiguradaAsync());


            // =====================================================
            // GESTIONAR CAJEROS
            // =====================================================

            GestionarCajerosCommand =
                new Command(
                    async () =>
                        await GestionarCajerosAsync());


            // =====================================================
            // NUEVO CAJERO
            // =====================================================

            NuevoCajeroCommand =
                new Command(
                    NuevoCajero);


            // =====================================================
            // GUARDAR
            // =====================================================

            GuardarCajeroCommand =
                new Command(
                    async () =>
                        await GuardarCajeroAsync());


            // =====================================================
            // CANCELAR
            // =====================================================

            CancelarCajeroCommand =
                new Command(
                    CancelarCajero);


            // =====================================================
            // EDITAR
            // =====================================================

            EditarCajeroCommand =
                new Command<Cajero>(
                    EditarCajero);


            // =====================================================
            // DESACTIVAR
            // =====================================================

            DesactivarCajeroCommand =
                new Command<Cajero>(
                    async cajero =>
                        await DesactivarCajeroAsync(cajero));


            // =====================================================
            // ACTIVAR
            // =====================================================

            ActivarCajeroCommand =
                new Command<Cajero>(
                    async cajero =>
                        await ActivarCajeroAsync(cajero));

            GestionarConfiguracionesCommand =
    new Command(
        async () =>
            await GestionarConfiguracionesAsync());


            NuevaConfiguracionCommand =
                new Command(
                    NuevaConfiguracion);


            GuardarConfiguracionCommand =
                new Command(
                    async () =>
                        await GuardarConfiguracionAsync());


            CancelarConfiguracionCommand =
                new Command(
                    CancelarConfiguracion);


            EditarConfiguracionCommand =
                new Command<ConfiguracionFacturacion>(
                    EditarConfiguracion);


            DesactivarConfiguracionCommand =
                new Command<ConfiguracionFacturacion>(
                    async configuracion =>
                        await DesactivarConfiguracionAsync(
                            configuracion));


            ActivarConfiguracionCommand =
                new Command<ConfiguracionFacturacion>(
                    async configuracion =>
                        await ActivarConfiguracionAsync(
                            configuracion));

            ExportarJornadaCommand =
    new Command<Jornada>(
        async jornada =>
            await ExportarJornadaAsync(jornada));
        }
        private async Task GestionarConfiguracionesAsync()
        {
            await CargarCajerosAsync();

            await CargarConfiguracionesAsync();

            MostrarGestionCajeros = false;

            MostrarFormularioCajero = false;

            MostrarConfiguracionNuevaJornada = false;

            MostrarGestionConfiguraciones = true;

            MostrarFormularioConfiguracion = false;

            Mensaje = string.Empty;
        }
        // =========================================================
        // EXPORTAR JORNADA
        // =========================================================

        private async Task ExportarJornadaAsync(
            Jornada? jornada)
        {
            if (jornada == null)
                return;

            if (Procesando)
                return;

            try
            {
                Procesando = true;

                Mensaje =
                    $"Generando Excel de la jornada #{jornada.Id}...";

                var ruta =
                    await _jornadaService
                        .ExportarJornadaExcelAsync(
                            jornada);

                // =====================================================
                // COMPARTIR / GUARDAR
                // =====================================================

                await Share.Default.RequestAsync(
                    new ShareFileRequest
                    {
                        Title =
                            $"Jornada #{jornada.Id}",

                        File = new ShareFile(ruta)
                    });

                Mensaje =
                    $"✅ Excel de la jornada #{jornada.Id} generado.";
            }
            catch (Exception ex)
            {
                Mensaje =
                    $"❌ No se pudo exportar la jornada:\n" +
                    $"{ex.Message}";
            }
            finally
            {
                Procesando = false;
            }
        }
        private void NuevaConfiguracion()
        {
            ConfiguracionEditando = null;

            SerieConfiguracion = string.Empty;

            TipoConfiguracion = "NORMAL";

            CajeroConfiguracion =
                Cajeros.FirstOrDefault();

            MostrarFormularioConfiguracion = true;

            Mensaje = string.Empty;
        }
        private void EditarConfiguracion(
    ConfiguracionFacturacion? configuracion)
        {
            if (configuracion == null)
                return;

            ConfiguracionEditando =
                configuracion;

            SerieConfiguracion =
                configuracion.Serie;

            TipoConfiguracion =
                configuracion.Tipo;

            CajeroConfiguracion =
                Cajeros.FirstOrDefault(
                    x =>
                        x.Id ==
                        configuracion.CajeroId);

            MostrarFormularioConfiguracion = true;

            Mensaje = string.Empty;
        }
        private void CancelarConfiguracion()
        {
            MostrarFormularioConfiguracion = false;

            ConfiguracionEditando = null;

            SerieConfiguracion = string.Empty;

            TipoConfiguracion = "NORMAL";

            CajeroConfiguracion = null;

            Mensaje = string.Empty;
        }
        private async Task GuardarConfiguracionAsync()
        {
            if (Procesando)
                return;

            var serie =
                SerieConfiguracion.Trim();

            var tipo =
                TipoConfiguracion
                    .Trim()
                    .ToUpperInvariant();

            // =========================================================
            // VALIDAR SERIE
            // =========================================================

            if (string.IsNullOrWhiteSpace(serie))
            {
                Mensaje =
                    "⚠️ Escribe la serie de facturación.";

                return;
            }

            // =========================================================
            // VALIDAR CAJERO
            // =========================================================

            if (CajeroConfiguracion == null)
            {
                Mensaje =
                    "⚠️ Selecciona un cajero.";

                return;
            }

            try
            {
                Procesando = true;

                // =====================================================
                // EDITAR
                // =====================================================

                if (ConfiguracionEditando != null)
                {
                    ConfiguracionEditando.CajeroId =
                        CajeroConfiguracion.Id;

                    ConfiguracionEditando.Serie =
                        serie;

                    ConfiguracionEditando.Tipo =
                        tipo;

                    await _configuracionFacturacionService
                        .ActualizarAsync(
                            ConfiguracionEditando);

                    Mensaje =
                        $"✅ Configuración " +
                        $"{ConfiguracionEditando.Serie} " +
                        $"actualizada.";
                }

                // =====================================================
                // CREAR
                // =====================================================

                else
                {
                    var configuracion =
                        await _configuracionFacturacionService
                            .CrearAsync(
                                CajeroConfiguracion.Id,
                                serie,
                                tipo);

                    Mensaje =
                        $"✅ Configuración " +
                        $"{configuracion.Serie} " +
                        $"creada.";
                }

                // =====================================================
                // LIMPIAR
                // =====================================================

                MostrarFormularioConfiguracion = false;

                ConfiguracionEditando = null;

                SerieConfiguracion = string.Empty;

                TipoConfiguracion = "NORMAL";

                CajeroConfiguracion = null;

                // =====================================================
                // RECARGAR
                // =====================================================

                await CargarConfiguracionesAsync();

                await CargarCajerosDisponiblesAsync();
            }
            catch (Exception ex)
            {
                Mensaje =
                    $"❌ No se pudo guardar la configuración:\n" +
                    $"{ex.Message}";
            }
            finally
            {
                Procesando = false;
            }
        }
        private async Task DesactivarConfiguracionAsync(
    ConfiguracionFacturacion? configuracion)
        {
            if (configuracion == null)
                return;

            if (Procesando)
                return;

            try
            {
                Procesando = true;

                await _configuracionFacturacionService
                    .DesactivarAsync(
                        configuracion);

                Mensaje =
                    $"✅ Configuración " +
                    $"{configuracion.Serie} desactivada.";

                await CargarConfiguracionesAsync();

                await CargarCajerosDisponiblesAsync();
            }
            catch (Exception ex)
            {
                Mensaje =
                    $"❌ No se pudo desactivar la configuración:\n" +
                    $"{ex.Message}";
            }
            finally
            {
                Procesando = false;
            }
        }
        private async Task CargarConfiguracionesAsync()
        {
            var configuraciones =
                await _configuracionFacturacionService
                    .ObtenerTodasAsync();

            ConfiguracionesFacturacion.Clear();

            foreach (var configuracion in configuraciones)
            {
                ConfiguracionesFacturacion.Add(
                    configuracion);
            }
        }
        private async Task ActivarConfiguracionAsync(
    ConfiguracionFacturacion? configuracion)
        {
            if (configuracion == null)
                return;

            if (Procesando)
                return;

            try
            {
                Procesando = true;

                await _configuracionFacturacionService
                    .ActivarAsync(
                        configuracion);

                Mensaje =
                    $"✅ Configuración " +
                    $"{configuracion.Serie} activada.";

                await CargarConfiguracionesAsync();

                await CargarCajerosDisponiblesAsync();
            }
            catch (Exception ex)
            {
                Mensaje =
                    $"❌ No se pudo activar la configuración:\n" +
                    $"{ex.Message}";
            }
            finally
            {
                Procesando = false;
            }
        }
        // =========================================================
        // INICIALIZAR
        // =========================================================

        public async Task InicializarAsync()
        {
            try
            {
                Procesando = true;

                Mensaje = string.Empty;

                MostrarConfiguracionNuevaJornada = false;

                MostrarFormularioCajero = false;

                await CargarCajerosAsync();

                await CargarConfiguracionesAsync();

                await CargarCajerosDisponiblesAsync();

                await CargarJornadaActualAsync();

                await CargarJornadasAsync();
            }
            catch (Exception ex)
            {
                Mensaje =
                    $"❌ Error cargando jornadas:\n" +
                    $"{ex.Message}";
            }
            finally
            {
                Procesando = false;
            }
        }


        // =========================================================
        // CARGAR CAJEROS
        // =========================================================

        private async Task CargarCajerosAsync()
        {
            var cajeros =
                await _cajeroService
                    .ObtenerTodosAsync();

            Cajeros.Clear();

            foreach (var cajero in cajeros)
            {
                Cajeros.Add(cajero);
            }
        }


        // =========================================================
        // GESTIONAR CAJEROS
        // =========================================================

        private async Task GestionarCajerosAsync()
        {
            await CargarCajerosAsync();

            MostrarConfiguracionNuevaJornada = false;

            MostrarFormularioCajero = false;

            MostrarGestionCajeros = true;

            Mensaje = string.Empty;
        }


        // =========================================================
        // NUEVO CAJERO
        // =========================================================

        private void NuevoCajero()
        {
            CajeroEditando = null;

            NombreCajero = string.Empty;

            MostrarFormularioCajero = true;

            Mensaje = string.Empty;
        }


        // =========================================================
        // EDITAR CAJERO
        // =========================================================

        private void EditarCajero(
            Cajero? cajero)
        {
            if (cajero == null)
                return;

            CajeroEditando = cajero;

            NombreCajero =
                cajero.Nombre;

            MostrarFormularioCajero = true;

            Mensaje = string.Empty;
        }


        // =========================================================
        // CANCELAR FORMULARIO
        // =========================================================

        private void CancelarCajero()
        {
            MostrarFormularioCajero = false;

            CajeroEditando = null;

            NombreCajero = string.Empty;

            Mensaje = string.Empty;
        }


        // =========================================================
        // GUARDAR CAJERO
        // =========================================================

        private async Task GuardarCajeroAsync()
        {
            if (Procesando)
                return;


            var nombre =
                NombreCajero.Trim();


            // =====================================================
            // VALIDAR
            // =====================================================

            if (string.IsNullOrWhiteSpace(nombre))
            {
                Mensaje =
                    "⚠️ Escribe el nombre del cajero.";

                return;
            }


            try
            {
                Procesando = true;


                // =================================================
                // EDITAR
                // =================================================

                if (CajeroEditando != null)
                {
                    CajeroEditando.Nombre =
                        nombre;

                    await _cajeroService
                        .ActualizarAsync(
                            CajeroEditando);

                    Mensaje =
                        $"✅ Cajero \"{nombre}\" actualizado.";
                }


                // =================================================
                // CREAR
                // =================================================

                else
                {
                    var cajero =
                        await _cajeroService
                            .CrearAsync(nombre);

                    Mensaje =
                        $"✅ Cajero \"{cajero.Nombre}\" creado.";
                }


                // =================================================
                // ACTUALIZAR
                // =================================================

                MostrarFormularioCajero = false;

                CajeroEditando = null;

                NombreCajero = string.Empty;

                await CargarCajerosAsync();

                await CargarCajerosDisponiblesAsync();
            }
            catch (Exception ex)
            {
                Mensaje =
                    $"❌ No se pudo guardar el cajero:\n" +
                    $"{ex.Message}";
            }
            finally
            {
                Procesando = false;
            }
        }


        // =========================================================
        // DESACTIVAR CAJERO
        // =========================================================

        private async Task DesactivarCajeroAsync(
            Cajero? cajero)
        {
            if (cajero == null)
                return;

            if (Procesando)
                return;


            try
            {
                Procesando = true;


                await _cajeroService
                    .DesactivarAsync(cajero);


                Mensaje =
                    $"✅ Cajero \"{cajero.Nombre}\" desactivado.";


                await CargarCajerosAsync();

                await CargarCajerosDisponiblesAsync();
            }
            catch (Exception ex)
            {
                Mensaje =
                    $"❌ No se pudo desactivar el cajero:\n" +
                    $"{ex.Message}";
            }
            finally
            {
                Procesando = false;
            }
        }


        // =========================================================
        // ACTIVAR CAJERO
        // =========================================================

        private async Task ActivarCajeroAsync(
            Cajero? cajero)
        {
            if (cajero == null)
                return;

            if (Procesando)
                return;


            try
            {
                Procesando = true;


                await _cajeroService
                    .ActivarAsync(cajero);


                Mensaje =
                    $"✅ Cajero \"{cajero.Nombre}\" activado.";


                await CargarCajerosAsync();

                await CargarCajerosDisponiblesAsync();
            }
            catch (Exception ex)
            {
                Mensaje =
                    $"❌ No se pudo activar el cajero:\n" +
                    $"{ex.Message}";
            }
            finally
            {
                Procesando = false;
            }
        }


        // =========================================================
        // CARGAR JORNADA ACTUAL
        // =========================================================

        private async Task CargarJornadaActualAsync()
        {
            JornadaActual =
                await _jornadaService
                    .ObtenerJornadaAbiertaAsync();


            if (JornadaActual == null)
            {
                CajerosJornada.Clear();

                return;
            }


            await CargarAsignacionesJornadaAsync();
        }


        // =========================================================
        // CARGAR ASIGNACIONES
        // =========================================================

        private async Task CargarAsignacionesJornadaAsync()
        {
            CajerosJornada.Clear();


            if (JornadaActual == null)
                return;


            var asignaciones =
                await _jornadaCajeroService
                    .ObtenerPorJornadaAsync(
                        JornadaActual);


            var cajeros =
                await _cajeroService
                    .ObtenerTodosAsync();


            var configuraciones =
                await _configuracionFacturacionService
                    .ObtenerActivasAsync();


            foreach (var asignacion in asignaciones)
            {
                var cajero =
                    cajeros.FirstOrDefault(
                        x =>
                            x.Id ==
                            asignacion.CajeroId);


                var configuracion =
                    configuraciones.FirstOrDefault(
                        x =>
                            x.Id ==
                            asignacion.ConfiguracionFacturacionId);


                if (cajero == null ||
                    configuracion == null)
                    continue;


                var item =
                    new JornadaCajeroItem(
                        cajero,
                        configuracion)
                    {
                        Seleccionado = true,

                        Asignacion =
                            asignacion,

                        NumeroFacturaInicialTexto =
                            asignacion
                                .NumeroFacturaInicial
                                .ToString()
                    };


                CajerosJornada.Add(item);
            }
        }


        // =========================================================
        // CARGAR CAJEROS DISPONIBLES
        // =========================================================

        private async Task CargarCajerosDisponiblesAsync()
        {
            var cajeros =
                await _cajeroService
                    .ObtenerTodosAsync();

            var configuraciones =
                await _configuracionFacturacionService
                    .ObtenerActivasAsync();

            CajerosDisponibles.Clear();

            // =========================================================
            // CADA CONFIGURACIÓN DE FACTURACIÓN ES UNA OPCIÓN
            // =========================================================

            foreach (var configuracion in configuraciones)
            {
                var cajero =
                    cajeros.FirstOrDefault(
                        x => x.Id == configuracion.CajeroId);

                if (cajero == null)
                    continue;

                CajerosDisponibles.Add(
                    new JornadaCajeroItem(
                        cajero,
                        configuracion));
            }
        }


        // =========================================================
        // CARGAR HISTORIAL
        // =========================================================

        private async Task CargarJornadasAsync()
        {
            var jornadas =
                await _jornadaService
                    .ObtenerTodasAsync();


            Jornadas.Clear();


            foreach (var jornada in jornadas)
            {
                Jornadas.Add(jornada);
            }
        }


        // =========================================================
        // ABRIR JORNADA
        // =========================================================

        private async Task AbrirJornadaAsync()
        {
            if (Procesando)
                return;


            var jornadaAbierta =
                await _jornadaService
                    .ObtenerJornadaAbiertaAsync();


            if (jornadaAbierta != null)
            {
                Mensaje =
                    "⚠️ Ya existe una jornada abierta.";


                JornadaActual =
                    jornadaAbierta;


                await CargarAsignacionesJornadaAsync();

                return;
            }


            await CargarCajerosDisponiblesAsync();


            foreach (var item in CajerosDisponibles)
            {
                item.Seleccionado = false;

                item.NumeroFacturaInicialTexto =
                    string.Empty;
            }


            MostrarGestionCajeros = false;

            MostrarConfiguracionNuevaJornada = true;

            Mensaje = string.Empty;
        }


        // =========================================================
        // INICIAR JORNADA CONFIGURADA
        // =========================================================

        private async Task IniciarJornadaConfiguradaAsync()
        {
            if (Procesando)
                return;


            var seleccionados =
                CajerosDisponibles
                    .Where(x =>
                        x.Seleccionado)
                    .ToList();


            if (seleccionados.Count == 0)
            {
                Mensaje =
                    "⚠️ Selecciona al menos un cajero.";

                return;
            }


            foreach (var item in seleccionados)
            {
                if (!int.TryParse(
                    item.NumeroFacturaInicialTexto,
                    out int numero))
                {
                    Mensaje =
                        $"⚠️ Ingresa la factura inicial " +
                        $"de {item.Nombre}.";

                    return;
                }


                if (numero < 0)
                {
                    Mensaje =
                        $"⚠️ La factura inicial de " +
                        $"{item.Nombre} no es válida.";

                    return;
                }
            }


            try
            {
                Procesando = true;


                // =================================================
                // CREAR JORNADA
                // =================================================

                var jornada =
                    await _jornadaService
                        .CrearJornadaAsync(
                            DateTime.Now);


                // =================================================
                // CREAR ASIGNACIONES
                // =================================================

                foreach (var item in seleccionados)
                {
                    var numero =
                        int.Parse(
                            item.NumeroFacturaInicialTexto);


                    await _jornadaCajeroService
                        .CrearAsync(
                            jornada,
                            item.Cajero,
                            item.Configuracion,
                            numero);
                }


                // =================================================
                // ACTUALIZAR UI
                // =================================================

                JornadaActual =
                    jornada;


                MostrarConfiguracionNuevaJornada =
                    false;


                MostrarGestionCajeros =
                    false;


                Mensaje =
                    $"✅ Jornada #{jornada.Id} iniciada.";


                await CargarAsignacionesJornadaAsync();

                await CargarJornadasAsync();
            }
            catch (Exception ex)
            {
                Mensaje =
                    $"❌ No se pudo iniciar la jornada:\n" +
                    $"{ex.Message}";
            }
            finally
            {
                Procesando = false;
            }
        }


        // =========================================================
        // CERRAR JORNADA
        // =========================================================

        private async Task CerrarJornadaAsync()
        {
            if (Procesando)
                return;


            if (JornadaActual == null)
            {
                Mensaje =
                    "⚠️ No hay una jornada abierta.";

                return;
            }


            try
            {
                Procesando = true;


                await _jornadaService
                    .CerrarJornadaAsync(
                        JornadaActual);


                Mensaje =
                    $"✅ Jornada #{JornadaActual.Id} cerrada.";


                JornadaActual = null;

                CajerosJornada.Clear();


                await CargarJornadasAsync();
            }
            catch (Exception ex)
            {
                Mensaje =
                    $"❌ No se pudo cerrar la jornada:\n" +
                    $"{ex.Message}";
            }
            finally
            {
                Procesando = false;
            }
        }
    }
}