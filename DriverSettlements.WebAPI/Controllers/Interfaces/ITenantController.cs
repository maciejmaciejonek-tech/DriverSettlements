using DriverSettlements.Common.DataContracts.DriverInterface;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces.Tenant;
using Microsoft.AspNetCore.Mvc;

namespace DriverSettlements.WebAPI.Controllers.Interfaces
{
    public interface ITenantController
    {
        IActionResult GetDrivers(GetDriversRequest request);
        IActionResult AddDriver(AddDriverRequest request);
        IActionResult UpdateDriver(UpdateDriverRequest request);
        IActionResult DeleteDriver(DeleteDriverRequest request);
        IActionResult SetDriverInvitationKey(SetDriverInvitationKeyRequest request);
        IActionResult GetTenantMetadata(GetTenantMetadataRequest request);
        IActionResult SaveTenantMetadata(SaveTenantMetadataRequest request);
        IActionResult DownloadExternalReport(DownloadExternalReportRequest request);
        IActionResult GetWeeklyExternalReportsMetadata(GetWeeklyExternalReportsMetadataRequest request);
        IActionResult UploadExternalReport(UploadExternalReportRequest request);
        IActionResult GetBasicReport(GetBasicReportRequest request);
        IActionResult GetCustomReport(GetCustomReportRequest request);
        IActionResult GetTenantFuelBills(GetTenantFuelBillsRequest request);
        IActionResult UpdateTenantFuelBill(UpdateTenantFuelBillRequest request);
        IActionResult GetDriverFuelBillsInfos(GetDriverFuelBillsInfosRequest request);
        IActionResult DownloadTenantFuelBillPhoto(DownloadTenantFuelBillPhotoRequest request);
        IActionResult GetTariffPlans(GetTariffPlansRequest request);
        IActionResult AddTariffPlan(AddTariffPlanRequest request);
        IActionResult UpdateTariffPlan(UpdateTariffPlanRequest request);
        IActionResult DeleteTariffPlan(DeleteTariffPlanRequest request);
        IActionResult GetTariffPlanAssignments(GetTariffPlanAssignmentsRequest request);
        IActionResult AddTariffPlanAssignment(AddTariffPlanAssignmentRequest request);
        IActionResult UpdateTariffPlanAssignment(UpdateTariffPlanAssignmentRequest request);
        IActionResult DeleteTariffPlanAssignment(DeleteTariffPlanAssignmentRequest request);
        IActionResult CopyTariffPlanAssignmentsFromPreviousWeek(CopyTariffPlanAssignmentsFromPreviousWeekRequest request);
        IActionResult GetSettlementItems(GetSettlementItemsRequest request);
        IActionResult AddSettlementItem(AddSettlementItemRequest request);
        IActionResult UpdateSettlementItem(UpdateSettlementItemRequest request);
        IActionResult DeleteSettlementItem(DeleteSettlementItemRequest request);
        IActionResult CopySettlementItemsFromPreviousWeek(CopySettlementItemsFromPreviousWeekRequest request);
        IActionResult GetSettlementTypes(GetSettlementTypesRequest request);
        IActionResult AddSettlementType(AddSettlementTypeRequest request);
        IActionResult UpdateSettlementType(UpdateSettlementTypeRequest request);
        IActionResult DeleteSettlementType(DeleteSettlementTypeRequest request);
        IActionResult ConfirmBillingWeek(ConfirmBillingWeekRequest request);
        IActionResult GetDriverDepositValue(GetDriverDepositValueRequest request);
        IActionResult Ping(string ping);
        IActionResult Test();
    }
}
