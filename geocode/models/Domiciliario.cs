using SQLite;

namespace geocode.models
{
    [Table("Domiciliarios")]
    public class Domiciliario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed, NotNull]
        public string Codigo { get; set; } = string.Empty;

        public string? Nombre { get; set; }

        public bool Activo { get; set; } = true;
    }
}