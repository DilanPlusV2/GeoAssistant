using SQLite;

namespace geocode.models
{
    [Table("FacturasAsignadas")]
    public class FacturaAsignada
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int JornadaId { get; set; }

        [Indexed]
        public int ConfiguracionFacturacionId { get; set; }

        [NotNull]
        public string NumeroFactura { get; set; } = string.Empty;

        public bool Utilizada { get; set; } = false;
    }
}