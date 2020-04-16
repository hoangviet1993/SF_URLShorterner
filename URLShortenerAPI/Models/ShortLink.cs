using Newtonsoft.Json;
using System;

namespace URLShortenerAPI.Models
{
    public class ShortLink
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string HashID { get; set; }

        public string Url { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
