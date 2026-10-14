using System.Collections.ObjectModel;
using System.Windows.Input;
using geocode.models;
using geocode.services;

namespace geocode.viewmodels
{
    public class EntregaViewModel : BindableObject
    {
        // =========================================================
        // SERVICIOS
        // =========================================================

        private readonly ProcesadorEntregaService _procesadorEntregaService;
        private readonly BebidaService _bebidaService;
        private readonly InventarioService _inventarioService;
        private readonly EntregaBebidaService _entregaBebidaService;
        private readonly JornadaService _jornadaService;
        private readonly DomiciliarioService _domiciliarioService;


        // =========================================================
        // COLECCIONES
        // =========================================================

        public ObservableCollection<Bebida> Bebidas { get; }
            = new();

        public ObservableCollection<Domiciliario> Domiciliarios { get; }
            = new();

        public ObservableCollection<Entrega> Entregas { get; }
            = new();


        // =========================================================
        // CAMPOS
        // =========================================================

        private string _numeroFactura = string.Empty;

        private string _codigoDomiciliario = string.Empty;

        private string _mensaje = string.Empty;

        private bool _mostrarBebidas;

        private bool _mostrarResultado;

        private bool _procesando;

        private Entrega? _entregaActual;

        private ResultadoEntrega? _resultadoActual;

        private bool _mostrarNuevoDomiciliario;

        private string _nuevoCodigoDomiciliario =
            string.Empty;

        private string _nuevoNombreDomiciliario =
            string.Empty;

        private string _mensajeDomiciliario = string.Empty;

        // =========================================================
        // PROPIEDADES
        // =========================================================
        public string MensajeDomiciliario
        {
            get => _mensajeDomiciliario;

            private set
            {
                if (_mensajeDomiciliario == value)
                    return;

                _mensajeDomiciliario = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(TieneMensajeDomiciliario));
            }
        }

        public bool TieneMensajeDomiciliario =>
            !string.IsNullOrWhiteSpace(MensajeDomiciliario);

        public bool MostrarNuevoDomiciliario
        {
            get => _mostrarNuevoDomiciliario;

            private set
            {
                if (_mostrarNuevoDomiciliario == value)
                    return;

                _mostrarNuevoDomiciliario = value;

                OnPropertyChanged();
            }
        }


        public string NuevoCodigoDomiciliario
        {
            get => _nuevoCodigoDomiciliario;

            set
            {
                if (_nuevoCodigoDomiciliario == value)
                    return;

                _nuevoCodigoDomiciliario = value;

                OnPropertyChanged();
            }
        }


        public string NuevoNombreDomiciliario
        {
            get => _nuevoNombreDomiciliario;

            set
            {
                if (_nuevoNombreDomiciliario == value)
                    return;

                _nuevoNombreDomiciliario = value;

                OnPropertyChanged();
            }
        }

        public string NumeroFactura
        {
            get => _numeroFactura;

            set
            {
                if (_numeroFactura == value)
                    return;

                _numeroFactura = value;

                OnPropertyChanged();
            }
        }


        public string CodigoDomiciliario
        {
            get => _codigoDomiciliario;

            private set
            {
                if (_codigoDomiciliario == value)
                    return;

                _codigoDomiciliario = value;

                OnPropertyChanged();

                OnPropertyChanged(
                    nameof(TieneDomiciliario));
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


        public bool MostrarBebidas
        {
            get => _mostrarBebidas;

            private set
            {
                if (_mostrarBebidas == value)
                    return;

                _mostrarBebidas = value;

                OnPropertyChanged();
            }
        }


        public bool MostrarResultado
        {
            get => _mostrarResultado;

            private set
            {
                if (_mostrarResultado == value)
                    return;

                _mostrarResultado = value;

                OnPropertyChanged();
            }
        }


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


        public Entrega? EntregaActual
        {
            get => _entregaActual;

            private set
            {
                if (_entregaActual == value)
                    return;

                _entregaActual = value;

                OnPropertyChanged();
            }
        }


        public ResultadoEntrega? ResultadoActual
        {
            get => _resultadoActual;

            private set
            {
                if (_resultadoActual == value)
                    return;

                _resultadoActual = value;

                OnPropertyChanged();
            }
        }


        // =========================================================
        // PROPIEDADES PARA LA INTERFAZ
        // =========================================================

        public bool TieneDomiciliario =>
            !string.IsNullOrWhiteSpace(
                CodigoDomiciliario);


        public bool ResultadoExitoso =>
            ResultadoActual?.Exitosa == true;


        public bool ResultadoFallido =>
            ResultadoActual != null &&
            !ResultadoActual.Exitosa;


        public string ResultadoFactura
        {
            get
            {
                if (ResultadoActual == null)
                    return string.Empty;

                var configuracion =
                    ResultadoActual.Configuracion;

                if (configuracion?.Tipo == "ELECTRONICA" &&
                    !string.IsNullOrWhiteSpace(
                        configuracion.Serie))
                {
                    return
                        $"{configuracion.Serie}-" +
                        $"{ResultadoActual.Factura?.NumeroFactura}";
                }

                return
                    ResultadoActual.Factura?
                        .NumeroFactura
                    ?? string.Empty;
            }
        }


        public string ResultadoCajero =>
            ResultadoActual?.Cajero?.Nombre
            ?? string.Empty;


        public string ResultadoDomiciliario =>
            ResultadoActual?.Domiciliario?.Codigo
            ?? string.Empty;


        // =========================================================
        // COMANDOS
        // =========================================================

        public ICommand RegistrarEntregaCommand { get; }

        public ICommand RegistrarBebidaCommand { get; }

        public ICommand NingunaBebidaCommand { get; }

        public ICommand SeleccionarDomiciliarioCommand { get; }

        public ICommand NuevoDomiciliarioCommand { get; }

        public ICommand GuardarDomiciliarioCommand { get; }

        public ICommand CancelarDomiciliarioCommand { get; }


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public EntregaViewModel(
            ProcesadorEntregaService procesadorEntregaService,
            BebidaService bebidaService,
            InventarioService inventarioService,
            EntregaBebidaService entregaBebidaService,
            JornadaService jornadaService,
            DomiciliarioService domiciliarioService)
        {
            _procesadorEntregaService =
                procesadorEntregaService;

            _bebidaService =
                bebidaService;

            _inventarioService =
                inventarioService;

            _entregaBebidaService =
                entregaBebidaService;

            _jornadaService =
                jornadaService;

            _domiciliarioService =
                domiciliarioService;


            // =====================================================
            // REGISTRAR ENTREGA
            // =====================================================

            RegistrarEntregaCommand =
                new Command(
                    async () =>
                        await RegistrarEntregaAsync());


            // =====================================================
            // REGISTRAR BEBIDA
            // =====================================================

            RegistrarBebidaCommand =
                new Command<Bebida>(
                    async bebida =>
                    {
                        if (bebida == null)
                            return;

                        await RegistrarBebidaAsync(
                            bebida);
                    });


            // =====================================================
            // NINGUNA BEBIDA
            // =====================================================

            NingunaBebidaCommand =
                new Command(
                    FinalizarSinBebida);


            // =====================================================
            // SELECCIONAR DOMICILIARIO
            // =====================================================

            SeleccionarDomiciliarioCommand =
                new Command<Domiciliario>(
                    SeleccionarDomiciliario);

            // =====================================================
            // NUEVO DOMICILIARIO
            // =====================================================

            NuevoDomiciliarioCommand =
                new Command(
                    MostrarFormularioDomiciliario);


            // =====================================================
            // GUARDAR DOMICILIARIO
            // =====================================================

            GuardarDomiciliarioCommand =
                new Command(
                    async () =>
                        await GuardarDomiciliarioAsync());


            // =====================================================
            // CANCELAR DOMICILIARIO
            // =====================================================

            CancelarDomiciliarioCommand =
                new Command(
                    CancelarNuevoDomiciliario);
        }


        // =========================================================
        // INICIALIZAR
        // =========================================================

        public async Task InicializarAsync()
        {
            try
            {
                await CargarBebidasAsync();

                await CargarDomiciliariosAsync();

                LimpiarEstado();
            }
            catch (Exception ex)
            {
                Mensaje =
                    $"❌ Error inicializando entregas:\n" +
                    $"{ex.Message}";

                MostrarResultado = true;
            }
        }


        // =========================================================
        // CARGAR BEBIDAS
        // =========================================================

        private async Task CargarBebidasAsync()
        {
            var bebidas =
                await _bebidaService
                    .ObtenerActivasAsync();

            Bebidas.Clear();

            foreach (var bebida in bebidas)
            {
                Bebidas.Add(bebida);
            }
        }


        // =========================================================
        // CARGAR DOMICILIARIOS
        // =========================================================

        private async Task CargarDomiciliariosAsync()
        {
            var domiciliarios =
                await _domiciliarioService
                    .ObtenerActivosAsync();

            Domiciliarios.Clear();

            foreach (var domiciliario in domiciliarios)
            {
                Domiciliarios.Add(
                    domiciliario);
            }
        }
        private void MostrarFormularioDomiciliario()
        {
            NuevoCodigoDomiciliario = string.Empty;

            NuevoNombreDomiciliario = string.Empty;

            MensajeDomiciliario = string.Empty;

            MostrarNuevoDomiciliario = true;
        }

        private void CancelarNuevoDomiciliario()
        {
            MostrarNuevoDomiciliario = false;

            NuevoCodigoDomiciliario = string.Empty;

            NuevoNombreDomiciliario = string.Empty;

            MensajeDomiciliario = string.Empty;
        }

        private async Task GuardarDomiciliarioAsync()
        {
            if (Procesando)
                return;


            // =====================================================
            // VALIDAR CÓDIGO
            // =====================================================

            if (string.IsNullOrWhiteSpace(
                NuevoCodigoDomiciliario))
            {
                Mensaje =
                    "Escribe el código del domiciliario.";

                MostrarResultado = true;

                return;
            }


            try
            {
                Procesando = true;


                var codigo =
                    NuevoCodigoDomiciliario
                        .Trim()
                        .ToUpperInvariant();


                var nombre =
                    string.IsNullOrWhiteSpace(
                        NuevoNombreDomiciliario)
                            ? null
                            : NuevoNombreDomiciliario.Trim();


                // =================================================
                // VERIFICAR SI YA EXISTE
                // =================================================

                var existe =
                    await _domiciliarioService
                        .ExisteCodigoAsync(codigo);


                if (existe)
                {
                    Mensaje =
                        $"Ya existe un domiciliario con " +
                        $"el código {codigo}.";

                    MostrarResultado = true;

                    return;
                }


                // =================================================
                // CREAR
                // =================================================

                var domiciliario =
                    await _domiciliarioService
                        .CrearAsync(
                            codigo,
                            nombre);


                // =================================================
                // ACTUALIZAR LISTA
                // =================================================

                await CargarDomiciliariosAsync();


                // =================================================
                // SELECCIONAR AUTOMÁTICAMENTE
                // =================================================

                CodigoDomiciliario =
                    domiciliario.Codigo;


                // =================================================
                // CERRAR FORMULARIO
                // =================================================

                MostrarNuevoDomiciliario =
                    false;


                NuevoCodigoDomiciliario =
                    string.Empty;

                NuevoNombreDomiciliario =
                    string.Empty;


                Mensaje =
                    $"✅ Domiciliario {domiciliario.Codigo} " +
                    $"registrado correctamente.";

                MostrarResultado = true;
            }
            catch (Exception ex)
            {
                Mensaje =
                    $"❌ No se pudo registrar el domiciliario.\n" +
                    $"{ex.Message}";

                MostrarResultado = true;
            }
            finally
            {
                Procesando = false;
            }
        }

        // =========================================================
        // SELECCIONAR DOMICILIARIO
        // =========================================================

        private void SeleccionarDomiciliario(
            Domiciliario? domiciliario)
        {
            if (domiciliario == null)
                return;

            CodigoDomiciliario =
                domiciliario.Codigo;
        }


        // =========================================================
        // REGISTRAR ENTREGA
        // =========================================================

        private async Task RegistrarEntregaAsync()
        {
            if (Procesando)
                return;


            // =====================================================
            // VALIDAR FACTURA
            // =====================================================

            if (string.IsNullOrWhiteSpace(
                NumeroFactura))
            {
                Mensaje =
                    "Escribe el número de factura.";

                MostrarResultado = true;

                OnPropertyChanged(
                    nameof(ResultadoExitoso));

                OnPropertyChanged(
                    nameof(ResultadoFallido));

                return;
            }


            // =====================================================
            // VALIDAR DOMICILIARIO
            // =====================================================

            if (string.IsNullOrWhiteSpace(
                CodigoDomiciliario))
            {
                Mensaje =
                    "Selecciona un domiciliario.";

                MostrarResultado = true;

                OnPropertyChanged(
                    nameof(ResultadoExitoso));

                OnPropertyChanged(
                    nameof(ResultadoFallido));

                return;
            }


            try
            {
                Procesando = true;

                MostrarResultado = false;

                MostrarBebidas = false;

                EntregaActual = null;

                ResultadoActual = null;

                Mensaje = string.Empty;


                // =================================================
                // CONSTRUIR TEXTO
                // =================================================

                var texto =
                    $"{NumeroFactura.Trim()}-" +
                    $"{CodigoDomiciliario.Trim()}";


                // =================================================
                // PROCESAR
                // =================================================

                var resultado =
                    await _procesadorEntregaService
                        .ProcesarAsync(texto);


                ResultadoActual =
                    resultado;

                Mensaje =
                    resultado.Mensaje;


                // =================================================
                // ACTUALIZAR UI
                // =================================================

                OnPropertyChanged(
                    nameof(ResultadoExitoso));

                OnPropertyChanged(
                    nameof(ResultadoFallido));

                OnPropertyChanged(
                    nameof(ResultadoFactura));

                OnPropertyChanged(
                    nameof(ResultadoCajero));

                OnPropertyChanged(
                    nameof(ResultadoDomiciliario));


                MostrarResultado = true;


                // =================================================
                // ERROR
                // =================================================

                if (!resultado.Exitosa)
                    return;


                // =================================================
                // GUARDAR ENTREGA
                // =================================================

                EntregaActual =
                    resultado.Entrega;


                // =================================================
                // MOSTRAR BEBIDAS
                // =================================================

                if (EntregaActual != null)
                {
                    MostrarBebidas = true;
                }


                // =================================================
                // LIMPIAR FACTURA
                // =================================================

                NumeroFactura =
                    string.Empty;
            }
            catch (Exception ex)
            {
                Mensaje =
                    $"Error registrando entrega:\n" +
                    $"{ex.Message}";

                MostrarResultado = true;
            }
            finally
            {
                Procesando = false;
            }
        }


        // =========================================================
        // REGISTRAR BEBIDA
        // =========================================================

        private async Task RegistrarBebidaAsync(
            Bebida bebida)
        {
            if (EntregaActual == null)
                return;

            if (Procesando)
                return;


            try
            {
                Procesando = true;


                // =================================================
                // OBTENER JORNADA
                // =================================================

                var jornada =
    ResultadoActual?.Jornada;

                if (jornada == null)
                {
                    Mensaje =
                        "❌ No se encontró la jornada de la entrega.";

                    MostrarResultado = true;

                    return;
                }


                // =================================================
                // INVENTARIO
                // =================================================

                var inventario =
                    await _inventarioService
                        .ObtenerOCrearAsync(
                            jornada);


                // =================================================
                // REGISTRAR BEBIDA
                // =================================================

                await _entregaBebidaService
                    .RegistrarAsync(
                        EntregaActual,
                        inventario,
                        bebida,
                        1);


                // =================================================
                // FINALIZAR
                // =================================================

                Mensaje =
                    $"{Mensaje}\n\n" +
                    $"🥤 {bebida.Nombre} registrada.";


                MostrarBebidas = false;

                EntregaActual = null;

                ResultadoActual = null;

                MostrarResultado = true;
            }
            catch (Exception ex)
            {
                Mensaje =
                    $"❌ No se pudo registrar la bebida.\n" +
                    $"{ex.Message}";

                MostrarResultado = true;
            }
            finally
            {
                Procesando = false;
            }
        }


        // =========================================================
        // SIN BEBIDA
        // =========================================================

        private void FinalizarSinBebida()
        {
            MostrarBebidas = false;

            EntregaActual = null;

            ResultadoActual = null;

            // El resultado permanece visible.
        }


        // =========================================================
        // LIMPIAR
        // =========================================================

        private void LimpiarEstado()
        {
            NumeroFactura = string.Empty;

            Mensaje = string.Empty;

            MostrarBebidas = false;

            MostrarResultado = false;

            Procesando = false;

            EntregaActual = null;

            ResultadoActual = null;

            // Importante:
            // NO limpiamos CodigoDomiciliario.
        }

    }
}