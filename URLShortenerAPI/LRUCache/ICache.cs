namespace URLShortenerAPI.LRUCache
{
    public interface ICache
    {
        int Count();

        string Get(string hashID);

        void Add(string hashID, string redirectUrl);
    }
}
