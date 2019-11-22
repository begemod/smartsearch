using System.Collections.Generic;

namespace Api.Features.ApartmentBuildings.Domain
{
    public class ApartmentBuildingEqualityComparer : IEqualityComparer<ApartmentBuilding>
    {
        public bool Equals(ApartmentBuilding x, ApartmentBuilding y) =>
            x != null && y != null && x.Id.Equals(y.Id);

        public int GetHashCode(ApartmentBuilding obj) =>
            obj.Id.GetHashCode();
    }
}