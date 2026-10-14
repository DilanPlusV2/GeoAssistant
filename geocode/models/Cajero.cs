using SQLite;

namespace geocode.models
{
    [Table("Cajeros")]
    public class Cajero
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Nombre { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }
}