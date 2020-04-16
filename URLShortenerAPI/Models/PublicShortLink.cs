using System;

namespace URLShortenerAPI.Models
{
    public class PublicShortLink
    {
        public string Url { get; set; }
        public string HashID { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
