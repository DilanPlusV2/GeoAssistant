using geocode.models;

namespace geocode.viewmodels
{
    public class JornadaCajeroItem : BindableObject
    {
        public Cajero Cajero { get; }

        public ConfiguracionFacturacion Configuracion { get; }

        public string Nombre =>
            Cajero.Nombre;

        public string Serie =>
            Configuracion.Serie;

        public string Tipo =>
            Configuracion.Tipo;

        public string NumeroFacturaInicialTexto
        {
            get => _numeroFacturaInicialTexto;
            set
            {
                if (_numeroFacturaInicialTexto == value)
                    return;

                _numeroFacturaInicialTexto = value;

                OnPropertyChanged();
            }
        }

        private string _numeroFacturaInicialTexto = string.Empty;


        public bool Seleccionado
        {
            get => _seleccionado;
            set
            {
                if (_seleccionado == value)
                    return;

                _seleccionado = value;

                OnPropertyChanged();
            }
        }

        private bool _seleccionado;


        public JornadaCajero? Asignacion { get; set; }


        public JornadaCajeroItem(
            Cajero cajero,
            ConfiguracionFacturacion configuracion)
        {
            Cajero = cajero;
            Configuracion = configuracion;
        }
    }
}