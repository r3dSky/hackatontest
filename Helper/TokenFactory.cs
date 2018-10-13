using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace hackatontest.Helper
{
    public static class TokenFactory
    {
        
        private static  string contentType = "application/x-www-form-urlencoded";
        private static string clientId = "4080eb88-b1ec-4ca1-9080-01a3556967e2";
        private static  string grantType = "client_credentials"; 
        private static  string tokenEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
        private static string clientSecret = "wlmxZ55|]:zzcELOMKV629}";
        private static string scope = "https%3A%2F%2Fgraph.microsoft.com%2F.default";
        private static string accessToken = null;
        private static string tokenForUser = null;
        private static System.DateTimeOffset expiration;

        public static async Task<string> Create()
        {
            return await getAccessTokenUsingPasswordGrant ();
        }
        private static async Task<string> getAccessTokenUsingPasswordGrant()
        {
            JObject jResult = null;
        
             String urlParameters = String.Format(
                    "grant_type={0}&scope={1}&client_id={2}&client_secret={3}",
                    grantType,
                    scope,
                    clientId,
                    clientSecret
            );

            HttpClient client = new HttpClient();
            var createBody = new StringContent(urlParameters, System.Text.Encoding.UTF8, contentType);
            HttpResponseMessage response = await client.PostAsync(tokenEndpoint, createBody);

            if (response.IsSuccessStatusCode)
            {
                Task<string> responseTask = response.Content.ReadAsStringAsync();
                responseTask.Wait();
                string responseContent = responseTask.Result;
                jResult = JObject.Parse(responseContent);
            }
            accessToken = (string)jResult["access_token"];

            if (!String.IsNullOrEmpty(accessToken))
            {
                //Set AuthenticationHelper values so that the regular MSAL auth flow won't be triggered.
                tokenForUser = accessToken;
                expiration = DateTimeOffset.UtcNow.AddHours(5);
            }

            return accessToken;
        }
        
    }
}