using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace www.ReadApi
{
    public class FilterAttribute
    {

        [JsonProperty("Category", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("Category")]
        public string? Category { get; set; }



        [JsonProperty("Attribute", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("Attribute")]
        public string? Attribute { get; set; }
    }
}