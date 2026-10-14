using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace geocode.models
{
    public class SincelejoData
    {
        [JsonPropertyName("sincelejo")]
        public List<Comuna>? Sincelejo { get; set; }
    }
}
