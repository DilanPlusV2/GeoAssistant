using geocode.models;

namespace geocode.models
{
    public class ResultadoEntrega
    {
        public bool Exitosa { get; set; }

        public string Mensaje { get; set; }
            = string.Empty;

        public Entrega? Entrega { get; set; }

        public FacturaAsignada? Factura { get; set; }

        public Cajero? Cajero { get; set; }

        public Domiciliario? Domiciliario { get; set; }

        public Jornada? Jornada { get; set; }

        public ConfiguracionFacturacion? Configuracion { get; set; }

        public static ResultadoEntrega Error(string mensaje)
        {
            return new ResultadoEntrega
            {
                Exitosa = false,
                Mensaje = mensaje
            };
        }

        public static ResultadoEntrega Exito(
            string mensaje,
            Entrega entrega,
            FacturaAsignada factura,
            Cajero cajero,
            Domiciliario domiciliario,
            Jornada jornada,
            ConfiguracionFacturacion configuracion)
        {
            return new ResultadoEntrega
            {
                Exitosa = true,
                Mensaje = mensaje,

                Entrega = entrega,
                Factura = factura,
                Cajero = cajero,
                Domiciliario = domiciliario,
                Jornada = jornada,
                Configuracion = configuracion
            };
        }
    }
}