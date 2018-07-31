using Microsoft.CustomerPreferences.Api.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Configuration;
using System.Net.Http;
using Headers = System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SampleCPMProject
{
    public class JarvisCPMClient : IJarvisCPMClient
    {
        private static readonly HttpClient httpClient;

        static JarvisCPMClient()
        {
            httpClient = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["BaseUrl"]) } ;

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<EmailContactPoint> GetEmailContactPoint(string email)
        {
            var reqMessage = new HttpRequestMessage(HttpMethod.Get, "api/EmailContacts");
            reqMessage.Headers.Add(HttpHeaders.EmailFilter, email);

            return await MakeRequestAndParseResponse<EmailContactPoint>(reqMessage);
        }

        public async Task<EmailContactPoint> PatchEmailContactPoint(EmailContactPoint contactToPatch)
        {
            var reqMessage = new HttpRequestMessage(new HttpMethod("PATCH"), "api/EmailContacts");
            reqMessage.Content = new StringContent(JsonConvert.SerializeObject(contactToPatch, new StringEnumConverter()));
            reqMessage.Content.Headers.ContentType = new Headers.MediaTypeHeaderValue("application/json");

            return await MakeRequestAndParseResponse<EmailContactPoint>(reqMessage);
        }

        public async Task<EmailContactabilitiesResponse> GetEmailContactability(EmailContactabilitiesRequest request)
        {
            var reqMessage = new HttpRequestMessage(HttpMethod.Post, "api/EmailContactabilities");
            reqMessage.Content = new StringContent(JsonConvert.SerializeObject(request, new StringEnumConverter()));
            reqMessage.Content.Headers.ContentType = new Headers.MediaTypeHeaderValue("application/json");

            return await MakeRequestAndParseResponse<EmailContactabilitiesResponse>(reqMessage);
        }

        public async Task<PhoneContactPoint> GetPhoneContactPoint(PhoneContactIdentity identity, bool useFuzzyMatch)
        {
            var matchingAlgo = useFuzzyMatch ? PhoneContactMatchStrategyType.PrioritizedNameElementFuzzyMatch : PhoneContactMatchStrategyType.ExactMatch;

            var reqMessage = new HttpRequestMessage(HttpMethod.Get, "api/PhoneContacts");
            reqMessage = AddNameToReqHeaders(identity.Name, reqMessage);
            reqMessage.Headers.Add(HttpHeaders.PhoneNumberFilter, identity.PhoneNumber);
            reqMessage.Headers.Add(HttpHeaders.MatchingAlgorithmFilter, matchingAlgo.ToString());

            return await MakeRequestAndParseResponse<PhoneContactPoint>(reqMessage);
        }

        public async Task<PhoneContactPoint> PatchPhoneContactPoint(PhoneContactPoint contactToPatch)
        {
            var reqMessage = new HttpRequestMessage(new HttpMethod("PATCH"), "api/PhoneContacts");
            reqMessage.Content = new StringContent(JsonConvert.SerializeObject(contactToPatch, new StringEnumConverter()));
            reqMessage.Content.Headers.ContentType = new Headers.MediaTypeHeaderValue("application/json");

            return await MakeRequestAndParseResponse<PhoneContactPoint>(reqMessage);
        }

        public async Task<PhoneContactabilitiesResponse> GetPhoneContactability(PhoneContactabilitiesRequest request)
        {
            var reqMessage = new HttpRequestMessage(HttpMethod.Post, "api/PhoneContactabilities");
            reqMessage.Content = new StringContent(JsonConvert.SerializeObject(request, new StringEnumConverter()));
            reqMessage.Content.Headers.ContentType = new Headers.MediaTypeHeaderValue("application/json");

            return await MakeRequestAndParseResponse<PhoneContactabilitiesResponse>(reqMessage);
        }

        private async Task<T> MakeRequestAndParseResponse<T>(HttpRequestMessage reqMessage)
        {
            reqMessage.Headers.Authorization = new Headers.AuthenticationHeaderValue("Bearer", AadAuthentication.GetAccessTokenForCpmApi());
            HttpResponseMessage response = await httpClient.SendAsync(reqMessage);

            await ValidateResponse(response);

            if (typeof(T) != typeof(object))
            {
                string responseAsJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseAsJson);
            }
            else
            {
                return default(T);
            }
        }

        private HttpRequestMessage AddNameToReqHeaders(ContactName identity, HttpRequestMessage reqMessage)
        {
            reqMessage.Headers.Add(HttpHeaders.FirstNameFilter, identity.FirstName);
            reqMessage.Headers.Add(HttpHeaders.MiddleNameFilter, identity.MiddleName);
            reqMessage.Headers.Add(HttpHeaders.LastNameFilter, identity.LastName);
            reqMessage.Headers.Add(HttpHeaders.GenerationalSuffixFilter, identity.GenerationalSuffix);

            return reqMessage;
        }

        private HttpRequestMessage AddAddressToReqHeaders(ContactMailingAddress address, HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Add(HttpHeaders.AddressUnitNumberFilter, address.UnitNumber);
            requestMessage.Headers.Add(HttpHeaders.AddressStreetLine1Filter, address.AddressLine1);
            requestMessage.Headers.Add(HttpHeaders.AddressStreetLine2Filter, address.AddressLine2);
            requestMessage.Headers.Add(HttpHeaders.AddressStreetLine3Filter, address.AddressLine3);
            requestMessage.Headers.Add(HttpHeaders.AddressDistrictFilter, address.District);
            requestMessage.Headers.Add(HttpHeaders.AddressCityFilter, address.City);
            requestMessage.Headers.Add(HttpHeaders.AddressStateFilter, address.State);
            requestMessage.Headers.Add(HttpHeaders.AddressCountryFilter, address.Country);
            requestMessage.Headers.Add(HttpHeaders.AddressPostalCodeFilter, address.PostalCode);

            return requestMessage;
        }

        private async Task ValidateResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string message;
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.BadRequest:
                        message = "Invalid contact point";
                        break;
                    case System.Net.HttpStatusCode.Unauthorized:
                    case System.Net.HttpStatusCode.Forbidden:
                        message = "Unauthorised to call CPM API";
                        break;
                    case System.Net.HttpStatusCode.NotFound:
                        message = "Contact Point not found in CPM";
                        break;
                    default:
                        message = $"An exception ocurred while calling CPM API: {await response.Content.ReadAsStringAsync()}";
                        break;
                }

                throw new ServiceException(message, response.StatusCode);
            }
        }
    }
}
