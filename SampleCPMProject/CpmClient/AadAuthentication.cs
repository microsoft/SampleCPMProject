using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace SampleCPMProject
{
    internal static class AadAuthentication
    {
        static AuthenticationContext context;
        static IClientAssertionCertificate credentials;

        static AadAuthentication()
        {
            X509Certificate2 cert = GetCertificateFromStore(ConfigurationManager.AppSettings["Thumbprint"]);
            context = new AuthenticationContext(ConfigurationManager.AppSettings["AadInstance"] + ConfigurationManager.AppSettings["TenantId"]);
            credentials = new ClientAssertionCertificate(ConfigurationManager.AppSettings["ClientId"], cert);
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

        private static X509Certificate2 GetCertificateFromStore(string certThumbprint)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

            var certs = store.Certificates.Find(X509FindType.FindByThumbprint, certThumbprint, false);

            if (certs.Count == 1)
            {
                return certs[0];
            }
            if (certs.Count > 1)
            {
                throw new System.Exception($"More than one certificate with thumbprint {certThumbprint} " +
                                    "found in LocalMachine store location");
            }

            throw new System.Exception($"No certificate found with thumbprint {certThumbprint} " +
                                "in LocalMachine store location");
        }
    }
}
