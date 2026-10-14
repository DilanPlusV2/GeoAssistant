using geocode.models;

namespace geocode.services
{
    public class EntregaParserService
    {
        public FacturaEntrada Parsear(
            string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return new FacturaEntrada
                {
                    EsValida = false,
                    Error = "La entrada está vacía."
                };
            }

            var partes = texto
                .Split(
                    '-',
                    StringSplitOptions.RemoveEmptyEntries
                )
                .Select(x => x.Trim())
                .ToList();

            // =====================================================
            // FACTURA NORMAL
            //
            // 26350 - D
            // =====================================================

            if (partes.Count == 2)
            {
                var numero = partes[0];
                var domiciliario = partes[1];

                if (!int.TryParse(numero, out _))
                {
                    return new FacturaEntrada
                    {
                        EsValida = false,
                        Error = "El número de factura no es válido."
                    };
                }

                if (domiciliario.Length != 1)
                {
                    return new FacturaEntrada
                    {
                        EsValida = false,
                        Error =
                            "El código del domiciliario no es válido."
                    };
                }

                return new FacturaEntrada
                {
                    NumeroFactura = numero,
                    CodigoDomiciliario =
                        domiciliario.ToUpperInvariant(),

                    Tipo = "NORMAL",
                    EsValida = true
                };
            }

            // =====================================================
            // FACTURA ELECTRÓNICA
            //
            // 007 - 8824 - D
            // =====================================================

            if (partes.Count == 3)
            {
                var serie = partes[0];
                var numero = partes[1];
                var domiciliario = partes[2];

                if (!int.TryParse(serie, out _))
                {
                    return new FacturaEntrada
                    {
                        EsValida = false,
                        Error = "La serie no es válida."
                    };
                }

                if (!int.TryParse(numero, out _))
                {
                    return new FacturaEntrada
                    {
                        EsValida = false,
                        Error =
                            "El número de factura electrónica no es válido."
                    };
                }

                if (domiciliario.Length != 1)
                {
                    return new FacturaEntrada
                    {
                        EsValida = false,
                        Error =
                            "El código del domiciliario no es válido."
                    };
                }

                return new FacturaEntrada
                {
                    Serie = serie.PadLeft(3, '0'),

                    NumeroFactura = numero,

                    CodigoDomiciliario =
                        domiciliario.ToUpperInvariant(),

                    Tipo = "ELECTRONICA",

                    EsValida = true
                };
            }

            return new FacturaEntrada
            {
                EsValida = false,
                Error =
                    "Formato no reconocido. Ejemplos: 26350 - D o 007 - 8824 - D."
            };
        }
    }
}