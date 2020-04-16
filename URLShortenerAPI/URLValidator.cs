namespace URLShortenerAPI
{
    using System;
    public class URLValidator
    {
        public static bool IsUrlValid(string url)
        {
            Uri uriResult;
            bool tryCreateResult = Uri.TryCreate(url, UriKind.Absolute, out uriResult);
            return tryCreateResult == true && uriResult != null ? true : false;
        }
    }
}
