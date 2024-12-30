using System.Text.Json.Serialization;
// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);


namespace www.Models.ApiResponse
{
    public class Categories
    {
        [JsonPropertyName("Types")]
        public List<Type> Types { get; set; }

        [JsonPropertyName("Errors")]
        public List<string> Errors { get; set; }
    }

    public class SubType
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Code")]
        public string Code { get; set; }

        [JsonPropertyName("SubTypes")]
        public List<object> SubTypes { get; set; }
    }

    public class Type
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Code")]
        public string Code { get; set; }

        [JsonPropertyName("SubTypes")]
        public List<SubType> SubTypes { get; set; }
    }

}