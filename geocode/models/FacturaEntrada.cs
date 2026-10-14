namespace geocode.models
{
    public class FacturaEntrada
    {
        public string NumeroFactura { get; set; } = string.Empty;

        public string? Serie { get; set; }

        public string? CodigoDomiciliario { get; set; }

        public string Tipo { get; set; } = "NORMAL";

        public bool EsValida { get; set; }

        public string? Error { get; set; }
    }
}