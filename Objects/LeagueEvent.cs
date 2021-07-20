using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueLoadout
{
    public class LeagueEvent
    {
        [JsonProperty(PropertyName = "data")]
        public JToken Data { get; set; }
        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; set; }
        [JsonProperty(PropertyName = "eventType")]
        public string EventType { get; set; }
    }
}
