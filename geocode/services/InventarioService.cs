using geocode.models;

namespace geocode.services
{
    public class InventarioService
    {
        private readonly DatabaseService _databaseService;

        public InventarioService(
            DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // =========================================================
        // OBTENER O CREAR INVENTARIO DE LA JORNADA
        // =========================================================

        public async Task<Inventario>
            ObtenerOCrearAsync(
                Jornada jornada)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            var inventario = await database
                .Table<Inventario>()
                .Where(x =>
                    x.JornadaId == jornada.Id &&
                    x.Estado == "ABIERTO")
                .FirstOrDefaultAsync();

            if (inventario != null)
                return inventario;

            inventario = new Inventario
            {
                JornadaId = jornada.Id,
                Estado = "ABIERTO",
                FechaApertura = DateTime.Now
            };

            await database.InsertAsync(inventario);

            return inventario;
        }

        // =========================================================
        // AGREGAR BEBIDA AL INVENTARIO
        // =========================================================

        public async Task<InventarioBebida>
            RegistrarStockInicialAsync(
                Inventario inventario,
                Bebida bebida,
                int cantidad)
        {
            if (cantidad < 0)
                throw new ArgumentException(
                    "La cantidad no puede ser negativa.");

            var database =
                await _databaseService.GetConnectionAsync();

            var existente = await database
                .Table<InventarioBebida>()
                .Where(x =>
                    x.InventarioId == inventario.Id &&
                    x.BebidaId == bebida.Id)
                .FirstOrDefaultAsync();

            if (existente != null)
            {
                throw new InvalidOperationException(
                    $"La bebida {bebida.Nombre} " +
                    "ya tiene inventario registrado.");
            }

            var inventarioBebida =
                new InventarioBebida
                {
                    InventarioId = inventario.Id,
                    BebidaId = bebida.Id,
                    StockInicial = cantidad,
                    StockActual = cantidad,
                    StockFinal = cantidad
                };

            await database.InsertAsync(
                inventarioBebida);

            return inventarioBebida;
        }

        // =========================================================
        // OBTENER STOCK DE UNA BEBIDA
        // =========================================================

        public async Task<InventarioBebida?>
            ObtenerStockAsync(
                Inventario inventario,
                Bebida bebida)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<InventarioBebida>()
                .Where(x =>
                    x.InventarioId == inventario.Id &&
                    x.BebidaId == bebida.Id)
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // OBTENER TODO EL INVENTARIO
        // =========================================================

        public async Task<List<InventarioBebida>>
            ObtenerInventarioAsync(
                Inventario inventario)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            return await database
                .Table<InventarioBebida>()
                .Where(x =>
                    x.InventarioId == inventario.Id)
                .ToListAsync();
        }

        // =========================================================
        // DESCONTAR STOCK
        // =========================================================

        public async Task DescontarAsync(
            Inventario inventario,
            Bebida bebida,
            int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException(
                    "La cantidad debe ser mayor que cero.");

            var database =
                await _databaseService.GetConnectionAsync();

            var stock = await database
                .Table<InventarioBebida>()
                .Where(x =>
                    x.InventarioId == inventario.Id &&
                    x.BebidaId == bebida.Id)
                .FirstOrDefaultAsync();

            if (stock == null)
            {
                throw new InvalidOperationException(
                    $"No existe inventario para " +
                    $"{bebida.Nombre}.");
            }

            if (stock.StockActual < cantidad)
            {
                throw new InvalidOperationException(
                    $"Stock insuficiente de " +
                    $"{bebida.Nombre}. " +
                    $"Disponible: {stock.StockActual}.");
            }

            stock.StockActual -= cantidad;

            await database.UpdateAsync(stock);
        }

        // =========================================================
        // CERRAR INVENTARIO
        // =========================================================

        public async Task CerrarAsync(
            Inventario inventario)
        {
            var database =
                await _databaseService.GetConnectionAsync();

            var bebidas = await database
                .Table<InventarioBebida>()
                .Where(x =>
                    x.InventarioId == inventario.Id)
                .ToListAsync();

            foreach (var bebida in bebidas)
            {
                bebida.StockFinal =
                    bebida.StockActual;

                await database.UpdateAsync(bebida);
            }

            inventario.Estado = "CERRADO";
            inventario.FechaCierre = DateTime.Now;

            await database.UpdateAsync(inventario);
        }
    }
}