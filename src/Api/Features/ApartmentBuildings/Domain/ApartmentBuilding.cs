using Newtonsoft.Json;

namespace Api.Features.ApartmentBuildings.Domain
{
    public class ApartmentBuilding
    {
        [JsonProperty("propertyID")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("formerName")]
        public string FormerName { get; set; }

        [JsonProperty("streetAddress")]
        public string StreetAddress { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("market")]
        public string Market { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }
}