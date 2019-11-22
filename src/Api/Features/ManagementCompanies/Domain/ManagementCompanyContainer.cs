using Newtonsoft.Json;

namespace Api.Features.ManagementCompanies.Domain
{
    public class ManagementCompanyContainer
    {
        [JsonProperty("mgmt")]
        public ManagementCompany Data { get; set; }
    }
}