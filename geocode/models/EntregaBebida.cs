using SQLite;

namespace geocode.models
{
    [Table("EntregaBebidas")]
    public class EntregaBebida
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int EntregaId { get; set; }

        [Indexed]
        public int BebidaId { get; set; }

        public int Cantidad { get; set; } = 1;
    }
}