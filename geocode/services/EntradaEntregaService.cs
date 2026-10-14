namespace geocode.services
{
    public class EntradaEntrega
    {
        public string? Serie { get; set; }

        public string NumeroFactura { get; set; } =
            string.Empty;

        public string? LetraDomiciliario { get; set; }

        public bool EsValida =>
            !string.IsNullOrWhiteSpace(NumeroFactura);
    }


    public class EntradaEntregaService
    {
        // =========================================================
        // INTERPRETAR ENTRADA
        // =========================================================

        public EntradaEntrega Interpretar(
            string texto)
        {
            var resultado =
                new EntradaEntrega();

            if (string.IsNullOrWhiteSpace(texto))
                return resultado;

            texto = texto
                .Trim()
                .ToUpperInvariant();

            // -----------------------------------------------------
            // Aceptar:
            //
            // 007-26350-D
            // 007 26350 D
            // 007 / 26350 / D
            // -----------------------------------------------------

            var partes = texto
                .Replace("-", " ")
                .Replace("/", " ")
                .Replace("_", " ")
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries);

            // -----------------------------------------------------
            // FORMATO:
            //
            // SERIE - FACTURA - LETRA
            // -----------------------------------------------------

            if (partes.Length >= 3)
            {
                resultado.Serie =
                    partes[0];

                resultado.NumeroFactura =
                    partes[1];

                resultado.LetraDomiciliario =
                    partes[2];

                return resultado;
            }

            // -----------------------------------------------------
            // FORMATO:
            //
            // FACTURA - LETRA
            // -----------------------------------------------------

            if (partes.Length == 2)
            {
                resultado.NumeroFactura =
                    partes[0];

                resultado.LetraDomiciliario =
                    partes[1];

                return resultado;
            }

            // -----------------------------------------------------
            // SOLO NÚMERO
            // -----------------------------------------------------

            if (partes.Length == 1)
            {
                resultado.NumeroFactura =
                    partes[0];

                return resultado;
            }

            return resultado;
        }
    }
}