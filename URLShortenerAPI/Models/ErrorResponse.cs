using Newtonsoft.Json;

namespace URLShortenerAPI.Models
{
    public class ErrorResponse
    {
        [JsonProperty(PropertyName = "details")]
        public string Details { get; set; }
    }
}
