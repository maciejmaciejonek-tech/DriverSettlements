using DriverSettlements.Common.DataContracts.MasterAdmin;
using DriverSettlements.Common.DataContracts.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Interfaces
{
    public interface IMasterAdmin
    {
        GetTenantsResponse GetTenants(GetTenantsRequest request);
        AddTenantResponse AddTenant(AddTenantRequest request);
        UpdateTenantResponse UpdateTenant(UpdateTenantRequest request);
        DeleteTenantResponse DeleteTenant(DeleteTenantRequest request);
        SetTenantInvitationKeyResponse SetTenantInvitationKey(SetTenantInvitationKeyRequest request);
        AddDefaultReportDefinitionsResponse AddDefaultReportDefinitions(AddDefaultReportDefinitionsRequest request);
        AddSettlementTypesResponse AddSettlementTypes(AddSettlementTypesRequest request);
        string Ping(string ping);
        string Test();
    }
}
