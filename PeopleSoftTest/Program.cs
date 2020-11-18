using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace PeopleSoftTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string result = await TestPeopleSoftEndpoint();
            Console.WriteLine(result);
            Console.Read();
        }

        private static async Task<string> TestPeopleSoftEndpoint()
        {
            HttpResponseMessage result = new HttpResponseMessage();
            HttpClient client = new HttpClient();

            TokenModel authenticatedToken = null;

            var values = new Dictionary<string, string>
            {
                {"grant_type", "cps" },
                {"client_id","thirdpartyapp" },
                {"client_secret", "v4secret" },
                {"scope", "openid profile offline_access thirdparty" },
                {"apikey", "ofHv09sm8HjfVy67FM8s/4r7L9lnmDbyPlWtvyTmChAhTPfN0Gptag==" }
            };

            var content = new FormUrlEncodedContent(values);

            try
            {
                Uri baseUri = new Uri("https://cityofcalgarytest.cps.golf/");
                Uri v4AuthUri = new Uri(baseUri, "identityapi/connect/token");
                var response = client.PostAsync(v4AuthUri.ToString(), content).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(responseString))
                {
                    authenticatedToken = JsonConvert.DeserializeObject<TokenModel>(responseString);
                }

                if(authenticatedToken != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticatedToken.access_token);

                    string queryString = "fromDate=2020-10-1&toDate=2020-11-11";

                    Uri apiUrl = new Uri(baseUri, $"thirdpartyapi/api/v1/PeopleSoft/GlData?{queryString}");

                    var contentReturn = await client.GetStringAsync(apiUrl);

                    return contentReturn;
                }

            }
            catch
            {
                
            }

            return "Error";
        }

        private class TokenModel
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
        }
    }
}
