using DriverSettlements.Common.DataContracts.MasterAdmin;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DriverSettlements.App.Services
{
    public class MasterAdminService : BaseService, IMasterAdminAsync
    {

        public MasterAdminService(IHttpsClientHandlerService service):base(service, "MasterAdmin")
        {
        }

        public async Task<GetTenantsResponse> GetTenantsAsync(GetTenantsRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetTenantsRequest, GetTenantsResponse>("GetTenants", request);
        }

        public async Task<AddTenantResponse> AddTenantAsync(AddTenantRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<AddTenantRequest, AddTenantResponse>("AddTenant", request);
        }

        public async Task<UpdateTenantResponse> UpdateTenantAsync(UpdateTenantRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<UpdateTenantRequest, UpdateTenantResponse>("UpdateTenant", request);
        }

        public async Task<DeleteTenantResponse> DeleteTenantAsync(DeleteTenantRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<DeleteTenantRequest, DeleteTenantResponse>("DeleteTenant", request);
        }

        public async Task<SetTenantInvitationKeyResponse> SetTenantInvitationKeyAsync(SetTenantInvitationKeyRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<SetTenantInvitationKeyRequest, SetTenantInvitationKeyResponse>("SetTenantInvitationKey", request);
        }
        public async Task<AddDefaultReportDefinitionsResponse> AddDefaultReportDefinitions(AddDefaultReportDefinitionsRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<AddDefaultReportDefinitionsRequest, AddDefaultReportDefinitionsResponse>("AddDefaultReportDefinitions", request);
        }
        public async Task<AddSettlementTypesResponse> AddSettlementTypes(AddSettlementTypesRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<AddSettlementTypesRequest, AddSettlementTypesResponse>("AddSettlementTypes", request);
        }
        public async Task<string> PingAsync(string ping)
        {
            try
            {
                return await PostAsync<string>("Ping", ping);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
