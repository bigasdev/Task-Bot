using System;
using System.Text;
using Newtonsoft.Json;

namespace discordbottemplate
{
    public struct ConfigJson{
        [JsonProperty("token")]
        public string Token{get;private set;}
        [JsonProperty("prefix")]
        public string CommandPrefix{get;private set;}
        [JsonProperty("debug")]
        public bool DebugMode{get;private set;}
    }
}
