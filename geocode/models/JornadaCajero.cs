using SQLite;

namespace geocode.models
{
    [Table("JornadaCajeros")]
    public class JornadaCajero
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int JornadaId { get; set; }

        [Indexed]
        public int CajeroId { get; set; }

        [Indexed]
        public int ConfiguracionFacturacionId { get; set; }

        // Primera factura con la que comienza
        // este cajero en esta jornada.
        [NotNull]
        public int NumeroFacturaInicial { get; set; }

        public bool Activo { get; set; } = true;
    }
}