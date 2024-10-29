using DriverSettlements.Common.DataContracts.MasterAdmin;
using DriverSettlements.Common.DataContracts.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Interfaces
{
    public interface IMasterAdminAsync
    {
        Task<GetTenantsResponse> GetTenantsAsync(GetTenantsRequest request);
        Task<AddTenantResponse> AddTenantAsync(AddTenantRequest request);
        Task<UpdateTenantResponse> UpdateTenantAsync(UpdateTenantRequest request);
        Task<DeleteTenantResponse> DeleteTenantAsync(DeleteTenantRequest request);
        Task<SetTenantInvitationKeyResponse> SetTenantInvitationKeyAsync(SetTenantInvitationKeyRequest request);
        Task<AddDefaultReportDefinitionsResponse> AddDefaultReportDefinitions(AddDefaultReportDefinitionsRequest request);
        Task<AddSettlementTypesResponse> AddSettlementTypes(AddSettlementTypesRequest request);
        Task<string> PingAsync(string ping);
    }
}
