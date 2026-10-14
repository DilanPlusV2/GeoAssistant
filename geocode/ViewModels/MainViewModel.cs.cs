using System.Collections.ObjectModel;
using geocode.models;
using geocode.services;
using Microsoft.Maui.Controls;

namespace geocode.viewmodels
{
    public class MainViewModel : BindableObject
    {
        private readonly DataService _dataService;
        private readonly GeoService _geoService;

        public ObservableCollection<Barrio> Barrios { get; set; } = new();
        public ObservableCollection<MessageItem> Mensajes { get; set; } = new();

        private string _textoEntrada = string.Empty;

        public string TextoEntrada
        {
            get => _textoEntrada;
            set
            {
                _textoEntrada = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            _dataService = new DataService();
            _geoService = new GeoService();

            CargarDatos();
        }

        // =========================================================
        // CARGAR BARRIOS
        // =========================================================

        private async void CargarDatos()
        {
            try
            {
                var data = await _dataService.LoadDataAsync();

                var todos = data.Comunas
                    .SelectMany(c => c.Barrios)
                    .Where(b => b.Lat != null && b.Lng != null)
                    .GroupBy(b => Limpiar(b.Nombre))
                    .Select(g => g.First())
                    .ToList();

                foreach (var barrio in todos)
                    Barrios.Add(barrio);

                AgregarRespuesta(
                    $"✅ {Barrios.Count} barrios cargados.\n" +
                    "Escribe un barrio o un comando para comenzar."
                );
            }
            catch (Exception ex)
            {
                AgregarRespuesta(
                    $"❌ Error cargando los barrios: {ex.Message}"
                );
            }
        }

        // =========================================================
        // ENVIAR MENSAJE
        // =========================================================

        public void EnviarMensaje()
        {
            if (string.IsNullOrWhiteSpace(TextoEntrada))
                return;

            var texto = TextoEntrada.Trim();

            // Mostrar mensaje del usuario
            Mensajes.Add(new MessageItem
            {
                Texto = texto,
                EsUsuario = true
            });

            // Limpiar caja
            TextoEntrada = string.Empty;

            // Procesar comando
            ProcesarComando(texto);
        }

        // =========================================================
        // PROCESAR COMANDO PRINCIPAL
        // =========================================================

        private void ProcesarComando(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return;

            var textoNormalizado = Limpiar(texto);

            // -----------------------------------------------------
            // 1. AGRUPACIÓN
            //
            // Ejemplo:
            //
            // esta la palma esta versalles
            // esta el pinar esta 20 de julio
            // -----------------------------------------------------

            if (EsComandoAgrupacion(textoNormalizado))
            {
                ProcesarAgrupacion(textoNormalizado);
                return;
            }

            // -----------------------------------------------------
            // 2. COMPARAR DOS BARRIOS
            //
            // Ejemplo:
            //
            // la palma con 7 de agosto
            // -----------------------------------------------------

            if (textoNormalizado.Contains(" con "))
            {
                ProcesarComparacion(textoNormalizado);
                return;
            }

            // -----------------------------------------------------
            // 3. UN SOLO BARRIO
            //
            // Ejemplo:
            //
            // la palma
            // -----------------------------------------------------

            ProcesarBarrio(textoNormalizado);
        }

        // =========================================================
        // DETECTAR AGRUPACIÓN
        // =========================================================

        private bool EsComandoAgrupacion(string texto)
        {
            if (texto.StartsWith("esta "))
                return true;

            return texto.Contains(" esta ");
        }

        // =========================================================
        // AGRUPAR BARRIOS
        // =========================================================

        private void ProcesarAgrupacion(string texto)
        {
            /*
             * Convertimos:
             *
             * esta la palma esta versalles esta el pinar
             *
             * en:
             *
             * la palma
             * versalles
             * el pinar
             */

            var textoModificado = texto;

            // Si comienza con "esta "
            if (textoModificado.StartsWith("esta "))
            {
                textoModificado = textoModificado.Substring(5);
            }

            // Los demás " esta " se convierten en separadores
            textoModificado = textoModificado.Replace(
                " esta ",
                "|"
            );

            var nombres = textoModificado
                .Split(
                    '|',
                    StringSplitOptions.RemoveEmptyEntries
                )
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            if (nombres.Count < 2)
            {
                // Si solamente hay un "esta", por ejemplo:
                //
                // "esta la palma"
                //
                // lo tratamos como búsqueda normal.

                var barrio = BuscarBarrio(nombres.FirstOrDefault() ?? "");

                if (barrio == null)
                {
                    AgregarRespuesta("❌ Barrio no encontrado.");
                    return;
                }

                MostrarBarriosCercanos(barrio);
                return;
            }

            var barriosEncontrados = nombres
                .Select(BuscarBarrio)
                .Where(b => b != null)
                .Distinct()
                .ToList();

            if (barriosEncontrados.Count == 0)
            {
                AgregarRespuesta(
                    "❌ No pude reconocer ninguno de los barrios."
                );

                return;
            }

            if (barriosEncontrados.Count == 1)
            {
                AgregarRespuesta(
                    $"⚠️ Solo pude reconocer: {barriosEncontrados[0].Nombre}"
                );

                return;
            }

            // -----------------------------------------------------
            // GENERAR COMBINACIONES
            // -----------------------------------------------------

            var combinaciones = new List<CombinacionBarrio>();

            for (int i = 0; i < barriosEncontrados.Count; i++)
            {
                for (int j = i + 1; j < barriosEncontrados.Count; j++)
                {
                    var barrio1 = barriosEncontrados[i];
                    var barrio2 = barriosEncontrados[j];

                    var distancia =
                        _geoService.CalcularDistancia(
                            barrio1,
                            barrio2
                        );

                    if (distancia <= 1)
                    {
                        combinaciones.Add(
                            new CombinacionBarrio
                            {
                                Barrio1 = barrio1,
                                Barrio2 = barrio2,
                                Distancia = distancia
                            }
                        );
                    }
                }
            }

            var ordenadas = combinaciones
                .OrderBy(x => x.Distancia)
                .ToList();

            var usados = new HashSet<Barrio>();

            var sugerencias = new List<string>();

            foreach (var item in ordenadas)
            {
                if (
                    usados.Contains(item.Barrio1) ||
                    usados.Contains(item.Barrio2)
                )
                {
                    continue;
                }

                usados.Add(item.Barrio1);
                usados.Add(item.Barrio2);

                sugerencias.Add(
                    $"📦 {item.Barrio1.Nombre} → {item.Barrio2.Nombre}\n" +
                    $"📏 {item.Distancia:F2} km"
                );
            }

            // -----------------------------------------------------
            // BARRIOS SIN PAREJA
            // -----------------------------------------------------

            var sinPareja = barriosEncontrados
                .Where(b => !usados.Contains(b))
                .ToList();

            foreach (var barrio in sinPareja)
            {
                sugerencias.Add(
                    $"⚠️ {barrio.Nombre} sin pedidos cercanos"
                );
            }

            AgregarRespuesta(
                "🚚 Sugerencias de agrupación:\n\n" +
                string.Join("\n\n", sugerencias)
            );
        }

        // =========================================================
        // COMPARAR DOS BARRIOS
        // =========================================================

        private void ProcesarComparacion(string texto)
        {
            var partes = texto.Split(
                new[] { " con " },
                StringSplitOptions.None
            );

            if (partes.Length != 2)
            {
                AgregarRespuesta(
                    "❌ No pude interpretar la comparación."
                );

                return;
            }

            var barrio1 = BuscarBarrio(partes[0]);
            var barrio2 = BuscarBarrio(partes[1]);

            if (barrio1 == null)
            {
                AgregarRespuesta(
                    $"❌ No encontré el barrio: {partes[0]}"
                );

                return;
            }

            if (barrio2 == null)
            {
                AgregarRespuesta(
                    $"❌ No encontré el barrio: {partes[1]}"
                );

                return;
            }

            var distancia =
                _geoService.CalcularDistancia(
                    barrio1,
                    barrio2
                );

            if (distancia <= 1.54)
            {
                AgregarRespuesta(
                    $"✅ Sí están cerca.\n\n" +
                    $"📍 {barrio1.Nombre}\n" +
                    $"📍 {barrio2.Nombre}\n" +
                    $"📏 Distancia: {distancia:F2} km"
                );
            }
            else
            {
                AgregarRespuesta(
                    $"❌ Están lejos.\n\n" +
                    $"📍 {barrio1.Nombre}\n" +
                    $"📍 {barrio2.Nombre}\n" +
                    $"📏 Distancia: {distancia:F2} km"
                );
            }
        }

        // =========================================================
        // BUSCAR UN SOLO BARRIO
        // =========================================================

        private void ProcesarBarrio(string texto)
        {
            var barrio = BuscarBarrio(texto);

            if (barrio == null)
            {
                AgregarRespuesta(
                    $"❌ No encontré el barrio \"{texto}\"."
                );

                return;
            }

            MostrarBarriosCercanos(barrio);
        }

        // =========================================================
        // MOSTRAR BARRIOS CERCANOS
        // =========================================================

        private void MostrarBarriosCercanos(Barrio barrio)
        {
            var cercanos = Barrios
                .Where(b => b != barrio)
                .Select(b => new
                {
                    Barrio = b,
                    Distancia =
                        _geoService.CalcularDistancia(
                            barrio,
                            b
                        )
                })
                .Where(x =>
                    x.Distancia > 0 &&
                    x.Distancia <= 1
                )
                .OrderBy(x => x.Distancia)
                .ToList();

            if (!cercanos.Any())
            {
                AgregarRespuesta(
                    $"📍 {barrio.Nombre}\n\n" +
                    "No hay barrios cercanos."
                );

                return;
            }

            var resultado =
                $"📍 Barrios cerca de {barrio.Nombre}:\n\n";

            resultado += string.Join(
                "\n",
                cercanos.Select(x =>
                    $"• {x.Barrio.Nombre} ({x.Distancia:F2} km)"
                )
            );

            AgregarRespuesta(resultado);
        }

        // =========================================================
        // BUSCAR BARRIO
        // =========================================================

        private Barrio? BuscarBarrio(string nombre)
        {
            var input = Limpiar(nombre);

            if (string.IsNullOrWhiteSpace(input))
                return null;

            // -----------------------------------------------------
            // 1. EXACTO
            // -----------------------------------------------------

            var exacto = Barrios.FirstOrDefault(
                b => Limpiar(b.Nombre) == input
            );

            if (exacto != null)
                return exacto;

            // -----------------------------------------------------
            // 2. EMPIEZA POR
            // -----------------------------------------------------

            var empieza = Barrios.FirstOrDefault(
                b => Limpiar(b.Nombre).StartsWith(input)
            );

            if (empieza != null)
                return empieza;

            // -----------------------------------------------------
            // 3. CONTIENE
            // -----------------------------------------------------

            var contiene = Barrios.FirstOrDefault(
                b => Limpiar(b.Nombre).Contains(input)
            );

            if (contiene != null)
                return contiene;

            // -----------------------------------------------------
            // 4. SIMILITUD
            // -----------------------------------------------------

            var mejor = Barrios
                .Select(b => new
                {
                    Barrio = b,
                    Score = Similaridad(
                        input,
                        Limpiar(b.Nombre)
                    )
                })
                .OrderByDescending(x => x.Score)
                .FirstOrDefault();

            if (mejor != null && mejor.Score >= 0.70)
                return mejor.Barrio;

            return null;
        }

        // =========================================================
        // LIMPIAR TEXTO
        // =========================================================

        private string Limpiar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            texto = texto
                .ToLowerInvariant()
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u")
                .Replace(",", " ")
                .Replace(".", " ")
                .Replace("/", " ")
                .Replace("-", " ");

            while (texto.Contains("  "))
                texto = texto.Replace("  ", " ");

            var palabras = texto
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries
                );

            var resultado = new List<string>();

            foreach (var palabra in palabras)
            {
                if (
                    resultado.Count == 0 ||
                    resultado.Last() != palabra
                )
                {
                    resultado.Add(palabra);
                }
            }

            return NormalizarFechas(
                string.Join(" ", resultado)
            );
        }

        // =========================================================
        // NORMALIZACIÓN DE FECHAS
        // =========================================================

        private string NormalizarFechas(string texto)
        {
            var correcciones = new Dictionary<string, string>
            {
                { "2 septiembre", "2 de septiembre" },
                { "20 enero", "20 de enero" },
                { "6 febrero", "6 de febrero" },
                { "7 agosto", "7 de agosto" },
                { "20 julio", "20 de julio" },
                { "28 mayo", "28 de mayo" },
                { "17 septiembre", "17 de septiembre" },
                { "6 enero", "6 de enero" },

                { "dos de septiembre", "2 de septiembre" },
                { "veinte de enero", "20 de enero" },
                { "seis de febrero", "6 de febrero" },
                { "siete de agosto", "7 de agosto" },
                { "veinte de julio", "20 de julio" },
                { "veintiocho de mayo", "28 de mayo" },
                { "diecisiete de septiembre", "17 de septiembre" },
                { "seis de enero", "6 de enero" },

                // Casos donde el reconocimiento anterior
                // convertía la fecha en números separados.
                { "2 9", "2 de septiembre" },
                { "20 1", "20 de enero" },
                { "6 2", "6 de febrero" },
                { "7 8", "7 de agosto" },
                { "20 7", "20 de julio" },
                { "28 5", "28 de mayo" },
                { "17 9", "17 de septiembre" },
                { "6 1", "6 de enero" }
            };

            foreach (var correccion in correcciones)
            {
                if (texto == correccion.Key)
                    return correccion.Value;
            }

            return texto;
        }

        // =========================================================
        // SIMILITUD
        // =========================================================

        private double Similaridad(string a, string b)
        {
            int distancia = Levenshtein(a, b);

            int maxLen = Math.Max(
                a.Length,
                b.Length
            );

            if (maxLen == 0)
                return 1;

            return 1.0 -
                ((double)distancia / maxLen);
        }

        private int Levenshtein(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;

            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; i++)
                d[i, 0] = i;

            for (int j = 0; j <= m; j++)
                d[0, j] = j;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int costo =
                        s[i - 1] == t[j - 1]
                            ? 0
                            : 1;

                    d[i, j] = Math.Min(
                        Math.Min(
                            d[i - 1, j] + 1,
                            d[i, j - 1] + 1
                        ),
                        d[i - 1, j - 1] + costo
                    );
                }
            }

            return d[n, m];
        }

        // =========================================================
        // HISTORIAL
        // =========================================================

        private void AgregarRespuesta(string texto)
        {
            Mensajes.Add(
                new MessageItem
                {
                    Texto = texto,
                    EsUsuario = false
                }
            );
        }

        // =========================================================
        // CLASE AUXILIAR
        // =========================================================

        private class CombinacionBarrio
        {
            public Barrio Barrio1 { get; set; } = null!;
            public Barrio Barrio2 { get; set; } = null!;
            public double Distancia { get; set; }
        }
    }
}
