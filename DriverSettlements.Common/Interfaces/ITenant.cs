using DriverSettlements.Common.DataContracts.DriverInterface;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces.Tenant;

namespace DriverSettlements.Common.Interfaces
{
    public interface ITenant
    {
        GetDriversResponse GetDrivers(GetDriversRequest request);
        AddDriverResponse AddDriver(AddDriverRequest request);
        UpdateDriverResponse UpdateDriver(UpdateDriverRequest request);
        DeleteDriverResponse DeleteDriver(DeleteDriverRequest request);
        SetDriverInvitationKeyResponse SetDriverInvitationKey(SetDriverInvitationKeyRequest request);
        GetTenantMetadataResponse GetTenantMetadata(GetTenantMetadataRequest request);
        SaveTenantMetadataResponse SaveTenantMetadata(SaveTenantMetadataRequest request);
        DownloadExternalReportResponse DownloadExternalReport(DownloadExternalReportRequest request);
        UploadExternalReportResponse UploadExternalReport(UploadExternalReportRequest request);
        GetWeeklyExternalReportsMetadataResponse GetWeeklyExternalReportsMetadata(GetWeeklyExternalReportsMetadataRequest request);
        GetBasicReportResponse GetBasicReport(GetBasicReportRequest request);
        GetCustomReportResponse GetCustomReport(GetCustomReportRequest request);
        GetTenantFuelBillsResponse GetTenantFuelBills(GetTenantFuelBillsRequest request);
        UpdateTenantFuelBillResponse UpdateTenantFuelBill(UpdateTenantFuelBillRequest request);
        GetDriverFuelBillsInfosResponse GetDriverFuelBillsInfos(GetDriverFuelBillsInfosRequest request);
        DownloadTenantFuelBillPhotoResponse DownloadTenantFuelBillPhoto(DownloadTenantFuelBillPhotoRequest request);
        GetTariffPlansResponse GetTariffPlans(GetTariffPlansRequest request);
        AddTariffPlanResponse AddTariffPlan(AddTariffPlanRequest request);
        UpdateTariffPlanResponse UpdateTariffPlan(UpdateTariffPlanRequest request);
        DeleteTariffPlanResponse DeleteTariffPlan(DeleteTariffPlanRequest request);
        GetTariffPlanAssignmentsResponse GetTariffPlanAssignments(GetTariffPlanAssignmentsRequest request);
        AddTariffPlanAssignmentResponse AddTariffPlanAssignment(AddTariffPlanAssignmentRequest request);
        UpdateTariffPlanAssignmentResponse UpdateTariffPlanAssignment(UpdateTariffPlanAssignmentRequest request);
        DeleteTariffPlanAssignmentResponse DeleteTariffPlanAssignment(DeleteTariffPlanAssignmentRequest request);
        CopyTariffPlanAssignmentsFromPreviousWeekResponse CopyTariffPlanAssignmentsFromPreviousWeek(CopyTariffPlanAssignmentsFromPreviousWeekRequest request);
        GetSettlementItemsResponse GetSettlementItems(GetSettlementItemsRequest request);
        AddSettlementItemResponse AddSettlementItem(AddSettlementItemRequest request);
        UpdateSettlementItemResponse UpdateSettlementItem(UpdateSettlementItemRequest request);
        DeleteSettlementItemResponse DeleteSettlementItem(DeleteSettlementItemRequest request);
        CopySettlementItemsFromPreviousWeekResponse CopySettlementItemsFromPreviousWeek(CopySettlementItemsFromPreviousWeekRequest request);
        GetSettlementTypesResponse GetSettlementTypes(GetSettlementTypesRequest request);
        AddSettlementTypeResponse AddSettlementType(AddSettlementTypeRequest request);
        UpdateSettlementTypeResponse UpdateSettlementType(UpdateSettlementTypeRequest request);
        DeleteSettlementTypeResponse DeleteSettlementType(DeleteSettlementTypeRequest request);
        ConfirmBillingWeekResponse ConfirmBillingWeek(ConfirmBillingWeekRequest request);
        GetDriverDepositValueResponse GetDriverDepositValue(GetDriverDepositValueRequest request);

        string Ping(string ping);
        string Test();
    }
}
