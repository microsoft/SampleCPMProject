using Microsoft.CustomerPreferences.Api.Contracts;
using System.Threading.Tasks;

namespace SampleCPMProject
{
    public interface IJarvisCPMClient
    {
        Task<EmailContactPoint> GetEmailContactPoint(string email);
        Task<EmailContactPoint> PatchEmailContactPoint(EmailContactPoint contactToPatch);
        Task<EmailContactabilitiesResponse> GetEmailContactability(EmailContactabilitiesRequest request);

        Task<PhoneContactPoint> GetPhoneContactPoint(PhoneContactIdentity identity, bool useFuzzyMatch);
        Task<PhoneContactPoint> PatchPhoneContactPoint(PhoneContactPoint contactToPatch);
        Task<PhoneContactabilitiesResponse> GetPhoneContactability(PhoneContactabilitiesRequest request);
    }
}
