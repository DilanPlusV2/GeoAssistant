using SQLite;

namespace geocode.models
{
    [Table("Inventarios")]
    public class Inventario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int JornadaId { get; set; }

        public string Estado { get; set; } = "ABIERTO";

        public DateTime FechaApertura { get; set; } = DateTime.Now;

        public DateTime? FechaCierre { get; set; }
    }
}