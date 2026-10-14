using SQLite;

namespace geocode.models
{
    [Table("InventarioBebidas")]
    public class InventarioBebida
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int InventarioId { get; set; }

        [Indexed]
        public int BebidaId { get; set; }

        public int StockInicial { get; set; }

        public int StockFinal { get; set; }

        public int StockActual { get; set; }
    }
}