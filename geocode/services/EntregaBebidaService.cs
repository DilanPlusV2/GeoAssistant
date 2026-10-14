using geocode.models;

namespace geocode.services
{
    public class EntregaBebidaService
    {
        private readonly DatabaseService _databaseService;
        private readonly InventarioService _inventarioService;

        public EntregaBebidaService(
            DatabaseService databaseService,
            InventarioService inventarioService)
        {
            _databaseService = databaseService;
            _inventarioService = inventarioService;
        }

        // =========================================================
        // REGISTRAR BEBIDA EN UNA ENTREGA
        // =========================================================

        public async Task<EntregaBebida>
            RegistrarAsync(
                Entrega entrega,
                Inventario inventario,
                Bebida bebida,
                int cantidad = 1)
        {
            if (cantidad <= 0)
                throw new ArgumentException(
                    "La cantidad debe ser mayor que cero.");

            var database =
                await _databaseService.GetConnectionAsync();

            // -----------------------------------------------------
            // Verificar si ya existe esta bebida en la entrega
            // -----------------------------------------------------

            var existente = await database
                .Table<EntregaBebida>()
                .Where(x =>
                    x.EntregaId == entrega.Id &&
                    x.BebidaId == bebida.Id)
                .FirstOrDefaultAsync();

            if (existente != null)
            {
                throw new InvalidOperationException(
                    $"La bebida {bebida.Nombre} " +
                    "ya está registrada en esta entrega.");
            }

            // -----------------------------------------------------
            // Descontar inventario
            // -----------------------------------------------------

            await _inventarioService.DescontarAsync(
                inventario,
                bebida,
                cantidad);

            // -----------------------------------------------------
            // Registrar bebida entregada
            // -----------------------------------------------------

            var entregaBebida =
                new EntregaBebida
                {
                    EntregaId = entrega.Id,
                    BebidaId = bebida.Id,
                    Cantidad = cantidad
                };

            await database.InsertAsync(
                entregaBebida);

            return entregaBebida;
        }

        // =========================================================
        // OBTENER BEBIDAS DE UNA ENTREGA
        // =========================================================

        public async Task<List<EntregaBebida>>
            ObtenerPorEntregaAsync(
                Entrega entrega)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<EntregaBebida>()
                .Where(x =>
                    x.EntregaId == entrega.Id)
                .ToListAsync();
        }
    }
}