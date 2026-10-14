using SQLite;

namespace geocode.models
{
    [Table("Bebidas")]
    public class Bebida
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Nombre { get; set; } = string.Empty;

        public bool Activa { get; set; } = true;
    }
}