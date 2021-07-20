using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueLoadout
{
    public class LeagueEvent
    {
        [JsonProperty(PropertyName = "uri")]
        public string uri;
        [JsonProperty(PropertyName = "eventType")]
        public string eventType;
    }
}
