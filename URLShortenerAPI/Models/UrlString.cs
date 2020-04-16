using System.ComponentModel.DataAnnotations;

namespace URLShortenerAPI.Models
{
    public class UrlString
    {
        [Required]
        public string url { get; set; }
    }
}
