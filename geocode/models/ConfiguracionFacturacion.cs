using SQLite;

namespace geocode.models
{
    [Table("ConfiguracionesFacturacion")]
    public class ConfiguracionFacturacion
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int CajeroId { get; set; }

        [NotNull]
        public string Serie { get; set; } = string.Empty;

        [NotNull]
        public string Tipo { get; set; } = "NORMAL";

        public bool Activa { get; set; } = true;
    }
}