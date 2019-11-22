using System.Collections.Generic;

namespace Api.Features.ManagementCompanies.Domain
{
    public class ManagementCompanyEqualityComparer : IEqualityComparer<ManagementCompany>
    {
        public bool Equals(ManagementCompany x, ManagementCompany y) =>
            x != null && y != null && x.Id.Equals(y.Id);

        public int GetHashCode(ManagementCompany obj) =>
            obj.Id.GetHashCode();
    }
}