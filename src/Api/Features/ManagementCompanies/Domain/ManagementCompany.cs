using Newtonsoft.Json;

namespace Api.Features.ManagementCompanies.Domain
{
    public class ManagementCompany
    {
        [JsonProperty("mgmtID")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("market")]
        public string Market { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }
}