using System.ComponentModel.DataAnnotations;

namespace URLShortenerAPI.Models
{
    public class UrlString
    {
        [Required]
        [MinLength(1)]
        [Url]
        public string url { get; set; }
    }
}
