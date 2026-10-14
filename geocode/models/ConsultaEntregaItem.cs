namespace geocode.models
{
    public class ConsultaEntregaItem
    {
        public int EntregaId { get; set; }

        public string NumeroFactura { get; set; }
            = string.Empty;

        public string Serie { get; set; }
            = string.Empty;

        public string TipoFactura { get; set; }
            = string.Empty;

        public string Cajero { get; set; }
            = string.Empty;

        public string Domiciliario { get; set; }
            = string.Empty;

        public bool Utilizada { get; set; }

        public string FacturaMostrada
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Serie))
                {
                    return $"{Serie}-{NumeroFactura}";
                }

                return NumeroFactura;
            }
        }

        public string Linea
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Domiciliario))
                {
                    return FacturaMostrada;
                }

                return $"{FacturaMostrada} ─ {Domiciliario}";
            }
        }
    }
}