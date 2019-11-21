using Nest;
using Newtonsoft.Json;

namespace Api.Features.ManagementCompanies.Domain
{
    [ElasticsearchType(IdProperty = nameof(MgmtId), RelationName = "mgmt")]
    public class ManagementCompanyData
    {
        [JsonProperty("mgmtID")]
        [Number(Name = "mgmtID")]
        public int MgmtId { get; set; }

        [JsonProperty("name")]
        [Text(Name = "name")]
        public string Name { get; set; }

        [JsonProperty("market")]
        [Keyword(Name = "market")]
        public string Market { get; set; }

        [JsonProperty("state")]
        [Keyword(Name = "state")]
        public string State { get; set; }
    }
}