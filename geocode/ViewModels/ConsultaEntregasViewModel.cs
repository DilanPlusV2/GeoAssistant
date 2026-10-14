using System.Collections.ObjectModel;
using System.Windows.Input;
using geocode.models;
using geocode.services;

namespace geocode.viewmodels
{
    public class ConsultaEntregasViewModel : BindableObject
    {
        // =========================================================
        // SERVICIOS
        // =========================================================

        private readonly ConsultaEntregaService
            _consultaService;

        private readonly JornadaService
            _jornadaService;


        // =========================================================
        // COLECCIONES
        // =========================================================

        public ObservableCollection<
            ConsultaEntregaItem> Entregas
        {
            get;
        } = new();


        public ObservableCollection<
            GrupoCajeroConsulta> Grupos
        {
            get;
        } = new();


        // =========================================================
        // CAMPOS
        // =========================================================

        private string _busqueda =
            string.Empty;

        private string _mensaje =
            string.Empty;

        private bool _cargando;


        // =========================================================
        // PROPIEDADES
        // =========================================================

        public string Busqueda
        {
            get => _busqueda;

            set
            {
                if (_busqueda == value)
                    return;

                _busqueda = value;

                OnPropertyChanged();

                Filtrar();
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


        public bool Cargando
        {
            get => _cargando;

            private set
            {
                if (_cargando == value)
                    return;

                _cargando = value;

                OnPropertyChanged();
            }
        }


        public bool HayResultados =>
            Grupos.Count > 0;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public ConsultaEntregasViewModel(
            ConsultaEntregaService consultaService,
            JornadaService jornadaService)
        {
            _consultaService =
                consultaService;

            _jornadaService =
                jornadaService;


            ActualizarCommand =
                new Command(
                    async () =>
                        await CargarAsync());
        }


        // =========================================================
        // COMANDO
        // =========================================================

        public ICommand ActualizarCommand
        {
            get;
        }


        // =========================================================
        // DATOS INTERNOS
        // =========================================================

        private List<ConsultaEntregaItem>
            _todasLasEntregas =
                new();


        // =========================================================
        // INICIALIZAR
        // =========================================================

        public async Task InicializarAsync()
        {
            await CargarAsync();
        }


        // =========================================================
        // CARGAR
        // =========================================================

        public async Task CargarAsync()
        {
            if (Cargando)
                return;


            try
            {
                Cargando = true;

                Mensaje = string.Empty;

                var jornada =
                    await _jornadaService
                        .ObtenerJornadaAbiertaAsync();


                if (jornada == null)
                {
                    _todasLasEntregas.Clear();

                    Grupos.Clear();

                    Mensaje =
                        "No hay una jornada abierta.";

                    OnPropertyChanged(
                        nameof(HayResultados));

                    return;
                }


                _todasLasEntregas =
    await _consultaService
        .ObtenerEntregasJornadaAsync(
            jornada);

                System.Diagnostics.Debug.WriteLine(
                    $"TOTAL ENTREGAS: {_todasLasEntregas.Count}");

                foreach (var entrega in _todasLasEntregas)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"ID={entrega.EntregaId} " +
                        $"FACTURA={entrega.NumeroFactura} " +
                        $"DOM={entrega.Domiciliario} " +
                        $"CAJERO={entrega.Cajero}");
                }

                Filtrar();
            }
            catch (Exception ex)
            {
                Mensaje =
                    $"Error consultando entregas:\n" +
                    $"{ex.Message}";
            }
            finally
            {
                Cargando = false;
            }
        }


        // =========================================================
        // FILTRAR
        // =========================================================

        private void Filtrar()
        {
            var texto =
                Normalizar(Busqueda);


            IEnumerable<ConsultaEntregaItem>
                filtradas;


            if (string.IsNullOrWhiteSpace(texto))
            {
                filtradas =
                    _todasLasEntregas;
            }
            else
            {
                filtradas =
                    _todasLasEntregas
                        .Where(x =>
                            Normalizar(
                                x.NumeroFactura)
                                .Contains(texto)

                            ||

                            Normalizar(
                                x.Serie)
                                .Contains(texto)

                            ||

                            Normalizar(
                                x.Cajero)
                                .Contains(texto)

                            ||

                            Normalizar(
                                x.Domiciliario)
                                .Contains(texto));
            }


            ConstruirGrupos(
                filtradas);


            OnPropertyChanged(
                nameof(HayResultados));
        }


        // =========================================================
        // AGRUPAR POR CAJERO
        // =========================================================

        private void ConstruirGrupos(
            IEnumerable<ConsultaEntregaItem>
                entregas)
        {
            Grupos.Clear();


            var grupos =
                entregas
                    .GroupBy(x =>
                        new
                        {
                            x.Cajero,
                            x.Serie
                        })
                    .OrderBy(x =>
                        x.Key.Cajero);


            foreach (var grupo in grupos)
            {
                var nuevo =
                    new GrupoCajeroConsulta
                    {
                        Cajero =
                            grupo.Key.Cajero,

                        Serie =
                            grupo.Key.Serie
                    };


                foreach (var entrega in grupo)
                {
                    nuevo.Entregas.Add(
                        entrega);
                }


                Grupos.Add(nuevo);
            }
        }


        // =========================================================
        // NORMALIZAR
        // =========================================================

        private string Normalizar(
            string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;


            return texto
                .Trim()
                .ToLowerInvariant()
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u")
                .Replace("ü", "u")
                .Replace("ñ", "n");
        }
    }


    // =============================================================
    // GRUPO DE CAJERO
    // =============================================================

    public class GrupoCajeroConsulta
    {
        public string Cajero { get; set; }
            = string.Empty;


        public string Serie { get; set; }
            = string.Empty;


        public ObservableCollection<
            ConsultaEntregaItem> Entregas
        {
            get;
        } = new();


        public string Titulo
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Serie))
                {
                    return Cajero;
                }

                return
                    $"{Cajero}  •  Serie {Serie}";
            }
        }
    }
}