using Microsoft.CustomerPreferences.Api.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleCPMProject
{
    public interface IJarvisCPMClient
    {
        Task<EmailContactPoint> GetEmailContactPoint(string email);
        Task<EmailContactPoint> PatchEmailContactPoint(EmailContactPoint contactToPatch);
        Task<EmailContactabilitiesResponse> GetEmailContactability(EmailContactabilitiesRequest request);

        Task<IEnumerable<PhoneContactPoint>> GetPhoneContactPoint(PhoneContactIdentity identity, bool useFuzzyMatch);
        Task PatchPhoneContactPoint(PhoneContactPoint contactToPatch);
        Task<PhoneContactabilitiesResponse> GetPhoneContactability(PhoneContactabilitiesRequest request);
    }
}
