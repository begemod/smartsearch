using Newtonsoft.Json;

namespace Api.Features.ManagementCompanies.Domain
{
    public class ManagementCompany
    {
        [JsonProperty("mgmt")]
        public ManagementCompanyData Data { get; set; }
    }
}