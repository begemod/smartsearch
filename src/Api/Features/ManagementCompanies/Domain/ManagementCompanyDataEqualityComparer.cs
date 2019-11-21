using System.Collections.Generic;

namespace Api.Features.ManagementCompanies.Domain
{
    public class ManagementCompanyDataEqualityComparer : IEqualityComparer<ManagementCompanyData>
    {
        public bool Equals(ManagementCompanyData x, ManagementCompanyData y) =>
            x != null && y != null && x.MgmtId.Equals(y.MgmtId);

        public int GetHashCode(ManagementCompanyData obj) =>
            obj.MgmtId.GetHashCode();
    }
}