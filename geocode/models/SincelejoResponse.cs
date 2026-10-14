using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace geocode.models
{
    public class SincelejoResponse
    {
        [JsonPropertyName("sincelejo")]
        public List<Comuna>? Comunas { get; set; }
    }
}
