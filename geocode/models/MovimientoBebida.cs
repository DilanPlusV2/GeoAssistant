using SQLite;

namespace geocode.models
{
    [Table("MovimientosBebida")]
    public class MovimientoBebida
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int InventarioId { get; set; }

        [Indexed]
        public int BebidaId { get; set; }

        [Indexed]
        public int? EntregaId { get; set; }

        [NotNull]
        public string Tipo { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;

        public string? Observacion { get; set; }
    }
}