namespace SampleCPMProject
{
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using System;
    using System.Configuration;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class CPMClientGenerator
    {
        private static readonly Lazy<string> aadTokenField = new Lazy<string>(() =>
        {
            ServicePointManager.ServerCertificateValidationCallback += (a, b, c, d) => true;
            AuthenticationContext authenticationContext = new AuthenticationContext(string.Concat(AadInstance, TenantId));
            ClientCredential credential = new ClientCredential(ClientId, ClientSecret);
            AuthenticationResult authenticationResult = authenticationContext.AcquireTokenAsync(AppResourceId, credential).Result;
            return authenticationResult.AccessToken;
        }, true);

        private static string AadInstance => ConfigurationManager.AppSettings["AadInstance"];

        private static string AadToken => aadTokenField.Value;

        private static string AppResourceId => ConfigurationManager.AppSettings["AppResourceId"];

        private static Uri BaseUrl => new Uri(ConfigurationManager.AppSettings["BaseUrl"]);

        private static string ClientId => ConfigurationManager.AppSettings["ClientId"];

        // NOTE: you will have to populate your client secret in App.Config
        private static string ClientSecret => ConfigurationManager.AppSettings["ClientSecret"];

        private static string TenantId => ConfigurationManager.AppSettings["TenantId"];

        public static Task<HttpClient> CreateHttpClientAsync()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = BaseUrl;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", AadToken);

            return Task.FromResult(httpClient);
        }
    }
}