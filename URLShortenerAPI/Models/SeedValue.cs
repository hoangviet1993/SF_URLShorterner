using Newtonsoft.Json;

namespace URLShortenerAPI.Models
{
    public class SeedValue
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public int Value { get; set; }
    }
}
