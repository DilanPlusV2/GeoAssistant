using SQLite;

namespace geocode.models
{
    [Table("Jornadas")]
    public class Jornada
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string FechaInicio { get; set; } = string.Empty;

        public string? FechaFin { get; set; }

        public string Estado { get; set; } = "ABIERTA";

        public string? Observacion { get; set; }
    }
}