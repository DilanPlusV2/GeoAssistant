using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace geocode.models
{
    public class Comuna
    {
        [JsonPropertyName("comuna")]
        public string? Nombre { get; set; }

        [JsonPropertyName("barrios")]
        public List<Barrio>? Barrios { get; set; }
    }
}
