using DriverSettlements.Common.DataContracts.DriverInterface;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Interfaces.Tenant;
using System.Diagnostics;

namespace DriverSettlements.App.Services
{
    public class TenantService : BaseService, ITenantAsync
    {

        public TenantService(IHttpsClientHandlerService service) : base(service, "Tenant")
        {
        }

        public async Task<GetDriversResponse> GetDriversAsync(GetDriversRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetDriversRequest, GetDriversResponse>("GetDrivers", request);
        }

        public async Task<AddDriverResponse> AddDriverAsync(AddDriverRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<AddDriverRequest, AddDriverResponse>("AddDriver", request);
        }

        public async Task<UpdateDriverResponse> UpdateDriverAsync(UpdateDriverRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<UpdateDriverRequest, UpdateDriverResponse>("UpdateDriver", request);
        }

        public async Task<DeleteDriverResponse> DeleteDriverAsync(DeleteDriverRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<DeleteDriverRequest, DeleteDriverResponse>("DeleteDriver", request);
        }

        public async Task<SetDriverInvitationKeyResponse> SetDriverInvitationKeyAsync(SetDriverInvitationKeyRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<SetDriverInvitationKeyRequest, SetDriverInvitationKeyResponse>("SetDriverInvitationKey", request);
        }

        public async Task<GetTenantMetadataResponse> GetTenantMetadataAsync(GetTenantMetadataRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetTenantMetadataRequest, GetTenantMetadataResponse>("GetTenantMetadata", request);
        }

        public async Task<SaveTenantMetadataResponse> SaveTenantMetadataAsync(SaveTenantMetadataRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<SaveTenantMetadataRequest, SaveTenantMetadataResponse>("SaveTenantMetadata", request);
        }

        public async Task<DownloadExternalReportResponse> DownloadExternalReportAsync(DownloadExternalReportRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<DownloadExternalReportRequest, DownloadExternalReportResponse>("DownloadExternalReport", request);
        }

        public async Task<UploadExternalReportResponse> UploadExternalReportAsync(UploadExternalReportRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<UploadExternalReportRequest, UploadExternalReportResponse>("UploadExternalReport", request);
        }

        public async Task<GetWeeklyExternalReportsMetadataResponse> GetWeeklyExternalReportsMetadataAsync(GetWeeklyExternalReportsMetadataRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetWeeklyExternalReportsMetadataRequest, GetWeeklyExternalReportsMetadataResponse>("GetWeeklyExternalReportsMetadata", request);
        }

        public async Task<AutomaticDriverRevisionResponse> AutomaticDriverRevisionAsync(AutomaticDriverRevisionRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<AutomaticDriverRevisionRequest, AutomaticDriverRevisionResponse>("AutomaticDriverRevision", request);
        }

        public async Task<GetBasicReportResponse> GetBasicReportAsync(GetBasicReportRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetBasicReportRequest, GetBasicReportResponse>("GetBasicReport", request);
        }

        public async Task<GetCustomReportResponse> GetCustomReportAsync(GetCustomReportRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetCustomReportRequest, GetCustomReportResponse>("GetCustomReport", request);
        }

        public async Task<GetTenantFuelBillsResponse> GetTenantFuelBillsAsync(GetTenantFuelBillsRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetTenantFuelBillsRequest, GetTenantFuelBillsResponse>("GetTenantFuelBills", request);
        }

        public async Task<UpdateTenantFuelBillResponse> UpdateTenantFuelBillAsync(UpdateTenantFuelBillRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<UpdateTenantFuelBillRequest, UpdateTenantFuelBillResponse>("UpdateTenantFuelBill", request);
        }

        public async Task<GetDriverFuelBillsInfosResponse> GetDriverFuelBillsInfosAsync(GetDriverFuelBillsInfosRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetDriverFuelBillsInfosRequest, GetDriverFuelBillsInfosResponse>("GetDriverFuelBillsInfos", request);
        }

        public async Task<DownloadTenantFuelBillPhotoResponse> DownloadTenantFuelBillPhotoAsync(DownloadTenantFuelBillPhotoRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<DownloadTenantFuelBillPhotoRequest, DownloadTenantFuelBillPhotoResponse>("DownloadTenantFuelBillPhoto", request);
        }

        public async Task<GetTariffPlansResponse> GetTariffPlansAsync(GetTariffPlansRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetTariffPlansRequest, GetTariffPlansResponse>("GetTariffPlans", request);
        }

        public async Task<AddTariffPlanResponse> AddTariffPlanAsync(AddTariffPlanRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<AddTariffPlanRequest, AddTariffPlanResponse>("AddTariffPlan", request);
        }

        public async Task<UpdateTariffPlanResponse> UpdateTariffPlanAsync(UpdateTariffPlanRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<UpdateTariffPlanRequest, UpdateTariffPlanResponse>("UpdateTariffPlan", request);
        }

        public async Task<DeleteTariffPlanResponse> DeleteTariffPlanAsync(DeleteTariffPlanRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<DeleteTariffPlanRequest, DeleteTariffPlanResponse>("DeleteTariffPlan", request);
        }

        public async Task<GetTariffPlanAssignmentsResponse> GetTariffPlanAssignmentsAsync(GetTariffPlanAssignmentsRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetTariffPlanAssignmentsRequest, GetTariffPlanAssignmentsResponse>("GetTariffPlanAssignments", request);
        }

        public async Task<AddTariffPlanAssignmentResponse> AddTariffPlanAssignmentAsync(AddTariffPlanAssignmentRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<AddTariffPlanAssignmentRequest, AddTariffPlanAssignmentResponse>("AddTariffPlanAssignment", request);
        }

        public async Task<UpdateTariffPlanAssignmentResponse> UpdateTariffPlanAssignmentAsync(UpdateTariffPlanAssignmentRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<UpdateTariffPlanAssignmentRequest, UpdateTariffPlanAssignmentResponse>("UpdateTariffPlanAssignment", request);
        }

        public async Task<DeleteTariffPlanAssignmentResponse> DeleteTariffPlanAssignmentAsync(DeleteTariffPlanAssignmentRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<DeleteTariffPlanAssignmentRequest, DeleteTariffPlanAssignmentResponse>("DeleteTariffPlanAssignment", request);
        }

        public async Task<CopyTariffPlanAssignmentsFromPreviousWeekResponse> CopyTariffPlanAssignmentsFromPreviousWeekAsync(CopyTariffPlanAssignmentsFromPreviousWeekRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<CopyTariffPlanAssignmentsFromPreviousWeekRequest, CopyTariffPlanAssignmentsFromPreviousWeekResponse>("CopyTariffPlanAssignmentsFromPreviousWeek", request);
        }

        public async Task<GetSettlementItemsResponse> GetSettlementItemsAsync(GetSettlementItemsRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetSettlementItemsRequest, GetSettlementItemsResponse>("GetSettlementItems", request);
        }

        public async Task<AddSettlementItemResponse> AddSettlementItemAsync(AddSettlementItemRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<AddSettlementItemRequest, AddSettlementItemResponse>("AddSettlementItem", request);
        }

        public async Task<UpdateSettlementItemResponse> UpdateSettlementItemAsync(UpdateSettlementItemRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<UpdateSettlementItemRequest, UpdateSettlementItemResponse>("UpdateSettlementItem", request);
        }

        public async Task<DeleteSettlementItemResponse> DeleteSettlementItemAsync(DeleteSettlementItemRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<DeleteSettlementItemRequest, DeleteSettlementItemResponse>("DeleteSettlementItem", request);
        }

        public async Task<CopySettlementItemsFromPreviousWeekResponse> CopySettlementItemsFromPreviousWeekAsync(CopySettlementItemsFromPreviousWeekRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<CopySettlementItemsFromPreviousWeekRequest, CopySettlementItemsFromPreviousWeekResponse>("CopySettlementItemsFromPreviousWeek", request);
        }

        public async Task<GetSettlementTypesResponse> GetSettlementTypesAsync(GetSettlementTypesRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetSettlementTypesRequest, GetSettlementTypesResponse>("GetSettlementTypes", request);
        }

        public async Task<AddSettlementTypeResponse> AddSettlementTypeAsync(AddSettlementTypeRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<AddSettlementTypeRequest, AddSettlementTypeResponse>("AddSettlementType", request);
        }

        public async Task<UpdateSettlementTypeResponse> UpdateSettlementTypeAsync(UpdateSettlementTypeRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<UpdateSettlementTypeRequest, UpdateSettlementTypeResponse>("UpdateSettlementType", request);
        }

        public async Task<DeleteSettlementTypeResponse> DeleteSettlementTypeAsync(DeleteSettlementTypeRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<DeleteSettlementTypeRequest, DeleteSettlementTypeResponse>("DeleteSettlementType", request);
        }

        public async Task<ConfirmBillingWeekResponse> ConfirmBillingWeekAsync(ConfirmBillingWeekRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<ConfirmBillingWeekRequest, ConfirmBillingWeekResponse>("ConfirmBillingWeek", request);
        }

        public async Task<GetDriverDepositValueResponse> GetDriverDepositValueAsync(GetDriverDepositValueRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetDriverDepositValueRequest, GetDriverDepositValueResponse>("GetDriverDepositValue", request);
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
