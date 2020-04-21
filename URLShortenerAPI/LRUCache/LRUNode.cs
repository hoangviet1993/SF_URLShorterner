using System;

namespace URLShortenerAPI.LRUCache
{
    public class LRUNode
    {
        public string HashID { get; set; }
        public string RedirectUrl { get; set; }
        public LRUNode Previous { get; set; }
        public LRUNode Next { get; set; }
        public LRUNode() {}
        public LRUNode(string inputHashID, string inputRedirectUrl)
        {
            if (string.IsNullOrEmpty(inputHashID) || string.IsNullOrWhiteSpace(inputHashID))
            {
                throw new ArgumentException("HashID cannot be null, empty or white spaces");
            }
            if (string.IsNullOrEmpty(inputRedirectUrl) || string.IsNullOrWhiteSpace(inputRedirectUrl))
            {
                throw new ArgumentException("Redirect URL cannot be null, empty or white spaces");
            }
            this.HashID = inputHashID;
            this.RedirectUrl = inputRedirectUrl;
        }
    }
}
