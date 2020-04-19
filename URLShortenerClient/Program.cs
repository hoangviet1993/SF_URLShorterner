using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

// Design largely duplicated from 
// https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
namespace URLShortenerClient
{
    public class URLInput
    {
        public string url { get; set; }
    }

    public class ShortLink
    {
        public string Url { get; set; }
        public string HashID { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    class Program
    {
        static HttpClientHandler handler = new HttpClientHandler()
        {
            AllowAutoRedirect = false
        };
        static HttpClient client = new HttpClient(handler);

        static async Task<ShortLink> CreateShortLinkPostJsonAsync(URLInput url)
        {
            string serializedUrl = JsonConvert.SerializeObject(url);
            StringContent encodedUrl = new StringContent(
                serializedUrl, UnicodeEncoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("api/links", encodedUrl);
            response.EnsureSuccessStatusCode();
            ShortLink shortLinkResponse = null;
            if (response != null)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                shortLinkResponse = JsonConvert.DeserializeObject<ShortLink>(responseString);
            }
            return shortLinkResponse;
        }

        static async Task<string> CreateInvalidUrlPostJson(URLInput url)
        {
            string serializedUrl = JsonConvert.SerializeObject(url);
            StringContent encodedUrl = new StringContent(
                serializedUrl, UnicodeEncoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("api/links", encodedUrl);
            string responseString = null;
            if (response != null)
            {
                responseString = await response.Content.ReadAsStringAsync();
            }
            return responseString;
        }

        static async Task<string> GetURLRedirectAsync(string hashID)
        {
            HttpResponseMessage response = await client.GetAsync($"api/links/{hashID}");
            return response.Headers.Location.AbsoluteUri;
        }

        static async Task<string> GetInvalidHashIDAsync(string hashID)
        {
            HttpResponseMessage response = await client.GetAsync($"api/links/{hashID}");
            string responseString = null;
            if (response != null)
            {
                responseString = await response.Content.ReadAsStringAsync();
            }
            return responseString;
        }

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
            Console.Read();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:8384/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                string validUrl = "http://www.google.com";
                URLInput url = new URLInput
                {
                    url = validUrl
                };
                ShortLink shortLinkResponse;
                shortLinkResponse = await CreateShortLinkPostJsonAsync(url);
                Console.WriteLine($"Posted Json and received  {JsonConvert.SerializeObject(shortLinkResponse)}");

                string hashID = shortLinkResponse.HashID;
                string expectedRedirectUrl = shortLinkResponse.Url;
                string redirectUrl = await GetURLRedirectAsync(hashID);
                Console.WriteLine($"Get {hashID}, expected {expectedRedirectUrl} and received {redirectUrl}");

                string invalidHashID = "abc";
                string invalidHashIDErrorResponse = await GetInvalidHashIDAsync(invalidHashID);
                Console.WriteLine($"Get invalid HashID and received {invalidHashIDErrorResponse}");

                string invalidUrl = "www.google.com";
                url = new URLInput
                {
                    url = invalidUrl
                };
                string urlErrorResponse = await CreateInvalidUrlPostJson(url);
                Console.WriteLine($"Posted invalid URL Json without protocal and received {urlErrorResponse}");

                invalidUrl = "http://.";
                url = new URLInput
                {
                    url = invalidUrl
                };
                urlErrorResponse = await CreateInvalidUrlPostJson(url);
                Console.WriteLine($"Posted invalid URL Json with protocol and received {urlErrorResponse}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
