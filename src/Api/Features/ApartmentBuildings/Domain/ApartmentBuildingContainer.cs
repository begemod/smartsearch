using Nest;
using Newtonsoft.Json;

namespace Api.Features.ApartmentBuildings.Domain
{
    public class ApartmentBuildingContainer
    {
        [JsonProperty("property")]
        [Object(Name = "prop")]
        public ApartmentBuilding Property { get; set; }
    }
}