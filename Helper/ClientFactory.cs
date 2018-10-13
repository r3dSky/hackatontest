using Microsoft.Graph;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
    
namespace hackatontest.Helper
{
    public static class ClientFactory
    {
        private static string clientId;
        private static string userName;
        private static  string password;
        private static  string contentType = "application/x-www-form-urlencoded";
        // Don't use password grant in your apps. Only use for legacy solutions and automated testing.
        private static  string grantType = "client_credentials"; 
        private static  string tokenEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
//        private static  string resourceId = "https%3A%2F%2Fgraph.microsoft.com%2F";
       // private static  string resourceId = "https://graph.microsoft.com/";

        private static string clientSecret = "wlmxZ55|]:zzcELOMKV629}";
//        private static string scope = "https://graph.microsoft.com/.default";
        private static string scope = "https%3A%2F%2Fgraph.microsoft.com%2F.default";


        private static string accessToken = null;
        private static string tokenForUser = null;
        private static System.DateTimeOffset expiration;

        private static GraphServiceClient graphClient = null;

        private static void CreateGraphService()
        {
            // Setup for CI
            clientId = "4080eb88-b1ec-4ca1-9080-01a3556967e2";
            userName = "roomTechPL";
            password = "hackathon1";

            GetAuthenticatedClient();
        }

        public static GraphServiceClient Create()
        {
            CreateGraphService();
            return graphClient;

        }

        // Get an access token and provide a GraphServiceClient.
        private static void GetAuthenticatedClient()
        {
            if (graphClient == null)
            {
                // Create Microsoft Graph client.
                try
                {
                    graphClient = new GraphServiceClient(
                        "https://graph.microsoft.com/v2.0",
                        new DelegateAuthenticationProvider(
                            async (requestMessage) =>
                            {
                                var token = await getAccessTokenUsingPasswordGrant();
                                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);

                            }));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Could not create a graph client: " + ex.Message);
                }
            }
        }

        private static async Task<string> getAccessTokenUsingPasswordGrant()
        {
            JObject jResult = null;
            // String urlParameters = String.Format(
            //         "grant_type={0}&resource={1}&client_id={2}&username={3}&password={4}",
            //         grantType,
            //         resourceId,
            //         clientId,
            //         userName,
            //         password
            // );
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