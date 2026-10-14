using SQLite;

namespace geocode.models
{
    [Table("Entregas")]
    public class Entrega
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int JornadaId { get; set; }

        [Indexed]
        public int ConfiguracionFacturacionId { get; set; }

        [Indexed]
        public int DomiciliarioId { get; set; }

        [NotNull]
        public string NumeroFactura { get; set; } = string.Empty;

        [NotNull]
        public string TipoFactura { get; set; } = "NORMAL";

        public string? Barrio { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;

        public string? Observacion { get; set; }
    }
}