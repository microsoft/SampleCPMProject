using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Configuration;

namespace SampleCPMProject
{
    internal static class AadAuthentication
    {
        static AuthenticationContext context;
        static ClientCredential credentials;

        static AadAuthentication()
        {
            context = new AuthenticationContext(ConfigurationManager.AppSettings["AadInstance"] + ConfigurationManager.AppSettings["TenantId"]);
            credentials = new ClientCredential(ConfigurationManager.AppSettings["ClientId"], ConfigurationManager.AppSettings["ClientSecret"]);
        }

        public static string GetAccessTokenForCpmApi()
        {
            byte retryCounter = 0;
            AuthenticationResult authenticationResult = null;
            do
            {
                try
                {
                    authenticationResult = context.AcquireTokenAsync(ConfigurationManager.AppSettings["AppResourceId"], credentials).Result;
                }
                catch (AdalException e)
                {
                    retryCounter++;
                    if (retryCounter == 3)
                    {
                        throw e;            //After 3 retries fail the operation.Exception would get logged by middleware.
                    }
                }
            } while (authenticationResult == null);

            return authenticationResult.AccessToken;
        }
    }
}
