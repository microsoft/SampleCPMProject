namespace SampleCPMProject
{
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class contains extension methods that the default http client does not support, such as a PATCH request, and a DELETE request with a body.
    /// </summary>
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> DeleteAsync<T>(this HttpClient httpClient, string relativePath, T model)
        {
            return httpClient.CustomMethodAsync(relativePath, model, "DELETE");
        }

        public static Task<HttpResponseMessage> PatchAsync<T>(this HttpClient httpClient, string relativePath, T model)
        {
            return httpClient.CustomMethodAsync(relativePath, model, "PATCH");
        }

        private static Task<HttpResponseMessage> CustomMethodAsync<T>(this HttpClient httpClient, string relativePath, T model, string methodType)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            if (relativePath == null)
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            HttpMethod method = new HttpMethod(methodType);
            HttpRequestMessage request = new HttpRequestMessage(method, relativePath)
            {
                Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json")
            };
            return httpClient.SendAsync(request);
        }
    }
}