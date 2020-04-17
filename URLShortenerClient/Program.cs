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

    public class ErrorResponse
    {
        public string details { get; set; }
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

        static async Task<ErrorResponse> CreateInvalidUrlPostJson(URLInput url)
        {
            string serializedUrl = JsonConvert.SerializeObject(url);
            StringContent encodedUrl = new StringContent(
                serializedUrl, UnicodeEncoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("api/links", encodedUrl);
            ErrorResponse errorResponse = null;
            if (response != null)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseString);
            }
            return errorResponse;
        }

        static async Task<ShortLink> CreateShortLinkPostForm(URLInput url)
        {
            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("url", url.url));
            FormUrlEncodedContent encodedForm = new FormUrlEncodedContent(keyValues);
            HttpResponseMessage response = await client.PostAsync("api/links", encodedForm);
            response.EnsureSuccessStatusCode();
            ShortLink shortLinkResponse = null;
            if (response != null)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                shortLinkResponse = JsonConvert.DeserializeObject<ShortLink>(responseString);
            }
            return shortLinkResponse;
        }

        static async Task<string> GetURLRedirectAsync(string hashID)
        {
            HttpResponseMessage response = await client.GetAsync($"api/links/{hashID}");
            return response.Headers.Location.AbsoluteUri;
        }

        static async Task<ErrorResponse> GetInvalidHashIDAsync(string hashID)
        {
            HttpResponseMessage response = await client.GetAsync($"api/links/{hashID}");
            ErrorResponse errorResponse = null;
            if (response != null)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseString);
            }
            return errorResponse;
        }

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:8384/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
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

                shortLinkResponse = await CreateShortLinkPostForm(url);
                Console.WriteLine($"Posted Form and received  {JsonConvert.SerializeObject(shortLinkResponse)}");

                string hashID = shortLinkResponse.HashID;
                string expectedRedirectUrl = shortLinkResponse.Url;
                string redirectUrl = await GetURLRedirectAsync(hashID);
                Console.WriteLine($"Get {hashID}, expected {expectedRedirectUrl} and received {redirectUrl}");

                string invalidHashID = "abc";
                ErrorResponse errorResponse = await GetInvalidHashIDAsync(invalidHashID);
                Console.WriteLine($"Get invalid HashID and received {JsonConvert.SerializeObject(errorResponse)}");

                string invalidUrl = "www.google.com";
                url = new URLInput
                {
                    url = invalidUrl
                };
                errorResponse = await CreateInvalidUrlPostJson(url);
                Console.WriteLine($"Posted invalid URL Json and received  {JsonConvert.SerializeObject(errorResponse)}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.Read();
        }
    }
}
