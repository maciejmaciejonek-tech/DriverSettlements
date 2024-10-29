using DriverSettlements.Common.DataContracts.DriverInterface;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces.Tenant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Interfaces
{
    public interface ITenantAsync
    {
        Task<GetDriversResponse> GetDriversAsync(GetDriversRequest request);
        Task<AddDriverResponse> AddDriverAsync(AddDriverRequest request);
        Task<UpdateDriverResponse> UpdateDriverAsync(UpdateDriverRequest request);
        Task<DeleteDriverResponse> DeleteDriverAsync(DeleteDriverRequest request);
        Task<SetDriverInvitationKeyResponse> SetDriverInvitationKeyAsync(SetDriverInvitationKeyRequest request);
        Task<GetTenantMetadataResponse> GetTenantMetadataAsync(GetTenantMetadataRequest request);
        Task<SaveTenantMetadataResponse> SaveTenantMetadataAsync(SaveTenantMetadataRequest request);
        Task<DownloadExternalReportResponse> DownloadExternalReportAsync(DownloadExternalReportRequest request);
        Task<UploadExternalReportResponse> UploadExternalReportAsync(UploadExternalReportRequest request);
        Task<GetWeeklyExternalReportsMetadataResponse> GetWeeklyExternalReportsMetadataAsync(GetWeeklyExternalReportsMetadataRequest request);
        Task<GetBasicReportResponse> GetBasicReportAsync(GetBasicReportRequest request);
        Task<GetCustomReportResponse> GetCustomReportAsync(GetCustomReportRequest request);
        Task<GetTenantFuelBillsResponse> GetTenantFuelBillsAsync(GetTenantFuelBillsRequest request);
        Task<UpdateTenantFuelBillResponse> UpdateTenantFuelBillAsync(UpdateTenantFuelBillRequest request);
        Task<GetDriverFuelBillsInfosResponse> GetDriverFuelBillsInfosAsync(GetDriverFuelBillsInfosRequest request);
        Task<DownloadTenantFuelBillPhotoResponse> DownloadTenantFuelBillPhotoAsync(DownloadTenantFuelBillPhotoRequest request);
        Task<GetTariffPlansResponse> GetTariffPlansAsync(GetTariffPlansRequest request);
        Task<AddTariffPlanResponse> AddTariffPlanAsync(AddTariffPlanRequest request);
        Task<UpdateTariffPlanResponse> UpdateTariffPlanAsync(UpdateTariffPlanRequest request);
        Task<DeleteTariffPlanResponse> DeleteTariffPlanAsync(DeleteTariffPlanRequest request);
        Task<GetTariffPlanAssignmentsResponse> GetTariffPlanAssignmentsAsync(GetTariffPlanAssignmentsRequest request);
        Task<AddTariffPlanAssignmentResponse> AddTariffPlanAssignmentAsync(AddTariffPlanAssignmentRequest request);
        Task<UpdateTariffPlanAssignmentResponse> UpdateTariffPlanAssignmentAsync(UpdateTariffPlanAssignmentRequest request);
        Task<DeleteTariffPlanAssignmentResponse> DeleteTariffPlanAssignmentAsync(DeleteTariffPlanAssignmentRequest request);
        Task<CopyTariffPlanAssignmentsFromPreviousWeekResponse> CopyTariffPlanAssignmentsFromPreviousWeekAsync(CopyTariffPlanAssignmentsFromPreviousWeekRequest request);
        Task<GetSettlementItemsResponse> GetSettlementItemsAsync(GetSettlementItemsRequest request);
        Task<AddSettlementItemResponse> AddSettlementItemAsync(AddSettlementItemRequest request);
        Task<UpdateSettlementItemResponse> UpdateSettlementItemAsync(UpdateSettlementItemRequest request);
        Task<DeleteSettlementItemResponse> DeleteSettlementItemAsync(DeleteSettlementItemRequest request);
        Task<CopySettlementItemsFromPreviousWeekResponse> CopySettlementItemsFromPreviousWeekAsync(CopySettlementItemsFromPreviousWeekRequest request);
        Task<GetSettlementTypesResponse> GetSettlementTypesAsync(GetSettlementTypesRequest request);
        Task<AddSettlementTypeResponse> AddSettlementTypeAsync(AddSettlementTypeRequest request);
        Task<UpdateSettlementTypeResponse> UpdateSettlementTypeAsync(UpdateSettlementTypeRequest request);
        Task<DeleteSettlementTypeResponse> DeleteSettlementTypeAsync(DeleteSettlementTypeRequest request);
        Task<ConfirmBillingWeekResponse> ConfirmBillingWeekAsync(ConfirmBillingWeekRequest request);
        Task<GetDriverDepositValueResponse> GetDriverDepositValueAsync(GetDriverDepositValueRequest request);

        Task<string> PingAsync(string ping);
    }

}

