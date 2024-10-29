using DriverSettlements.Common.DataContracts.DriverInterface;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces.Tenant;
using DriverSettlements.Database;
using DriverSettlements.WebAPI.Configuration;
using DriverSettlements.WebAPI.Controllers.Interfaces;
using DriverSettlements.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DriverSettlements.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase, ITenantController
    {
        private readonly TenantService _service;
        private readonly ILogger<TenantController> _logger;

        public TenantController(DatabaseContext dbContext, ILogger<TenantController> logger, ILogger<TenantService> tenantServiceLogger, IOptions<ExternalReportsConfiguration> externalReportsConfiguration, IOptions<PhotosConfiguration> photosConfiguration)
        {
            _logger = logger;
            _service = new TenantService(dbContext, tenantServiceLogger, externalReportsConfiguration, photosConfiguration);
        }

        [HttpPost("GetDrivers")]
        public IActionResult GetDrivers([FromBody] GetDriversRequest request)
        {
            try
            {
                return Ok(_service.GetDrivers(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetDriversResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("AddDriver")]
        public IActionResult AddDriver([FromBody] AddDriverRequest request)
        {
            try
            {
                return Ok(_service.AddDriver(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new AddDriverResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("UpdateDriver")]
        public IActionResult UpdateDriver([FromBody] UpdateDriverRequest request)
        {
            try
            {
                return Ok(_service.UpdateDriver(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new UpdateDriverResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("DeleteDriver")]
        public IActionResult DeleteDriver([FromBody] DeleteDriverRequest request)
        {
            try
            {
                return Ok(_service.DeleteDriver(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new DeleteDriverResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("SetDriverInvitationKey")]
        public IActionResult SetDriverInvitationKey([FromBody] SetDriverInvitationKeyRequest request)
        {
            try
            {
                return Ok(_service.SetDriverInvitationKey(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new SetDriverInvitationKeyResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("GetTenantMetadata")]
        public IActionResult GetTenantMetadata(GetTenantMetadataRequest request)
        {
            try
            {
                return Ok(_service.GetTenantMetadata(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetTenantMetadataResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("SaveTenantMetadata")]
        public IActionResult SaveTenantMetadata(SaveTenantMetadataRequest request)
        {
            try
            {
                return Ok(_service.SaveTenantMetadata(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new SaveTenantMetadataResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("DownloadExternalReport")]
        public IActionResult DownloadExternalReport(DownloadExternalReportRequest request)
        {
            try
            {
                return Ok(_service.DownloadExternalReport(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new DownloadExternalReportResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("UploadExternalReport")]
        public IActionResult UploadExternalReport(UploadExternalReportRequest request)
        {
            try
            {
                return Ok(_service.UploadExternalReport(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new UploadExternalReportResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("GetWeeklyExternalReportsMetadata")]
        public IActionResult GetWeeklyExternalReportsMetadata(GetWeeklyExternalReportsMetadataRequest request)
        {
            try
            {
                return Ok(_service.GetWeeklyExternalReportsMetadata(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetWeeklyExternalReportsMetadataResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("GetBasicReport")]
        public IActionResult GetBasicReport(GetBasicReportRequest request)
        {
            try
            {
                return Ok(_service.GetBasicReport(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetBasicReportResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("GetCustomReport")]
        public IActionResult GetCustomReport(GetCustomReportRequest request)
        {
            try
            {
                return Ok(_service.GetCustomReport(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetCustomReportResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("GetTenantFuelBills")]
        public IActionResult GetTenantFuelBills(GetTenantFuelBillsRequest request)
        {
            try
            {
                return Ok(_service.GetTenantFuelBills(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetTenantFuelBillsResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("UpdateTenantFuelBill")]
        public IActionResult UpdateTenantFuelBill([FromBody] UpdateTenantFuelBillRequest request)
        {
            try
            {
                return Ok(_service.UpdateTenantFuelBill(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new UpdateTenantFuelBillResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("GetDriverFuelBillsInfos")]
        public IActionResult GetDriverFuelBillsInfos(GetDriverFuelBillsInfosRequest request)
        {
            try
            {
                return Ok(_service.GetDriverFuelBillsInfos(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetDriverFuelBillsInfosResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("DownloadTenantFuelBillPhoto")]
        public IActionResult DownloadTenantFuelBillPhoto(DownloadTenantFuelBillPhotoRequest request)
        {
            try
            {
                return Ok(_service.DownloadTenantFuelBillPhoto(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new DownloadTenantFuelBillPhotoResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("GetTariffPlans")]
        public IActionResult GetTariffPlans([FromBody] GetTariffPlansRequest request)
        {
            try
            {
                return Ok(_service.GetTariffPlans(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetTariffPlansResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("AddTariffPlan")]
        public IActionResult AddTariffPlan([FromBody] AddTariffPlanRequest request)
        {
            try
            {
                return Ok(_service.AddTariffPlan(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new AddTariffPlanResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("UpdateTariffPlan")]
        public IActionResult UpdateTariffPlan([FromBody] UpdateTariffPlanRequest request)
        {
            try
            {
                return Ok(_service.UpdateTariffPlan(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new UpdateTariffPlanResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("DeleteTariffPlan")]
        public IActionResult DeleteTariffPlan([FromBody] DeleteTariffPlanRequest request)
        {
            try
            {
                return Ok(_service.DeleteTariffPlan(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new DeleteTariffPlanResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("GetTariffPlanAssignments")]
        public IActionResult GetTariffPlanAssignments([FromBody] GetTariffPlanAssignmentsRequest request)
        {
            try
            {
                return Ok(_service.GetTariffPlanAssignments(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetTariffPlanAssignmentsResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("AddTariffPlanAssignment")]
        public IActionResult AddTariffPlanAssignment([FromBody] AddTariffPlanAssignmentRequest request)
        {
            try
            {
                return Ok(_service.AddTariffPlanAssignment(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new AddTariffPlanAssignmentResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("UpdateTariffPlanAssignment")]
        public IActionResult UpdateTariffPlanAssignment([FromBody] UpdateTariffPlanAssignmentRequest request)
        {
            try
            {
                return Ok(_service.UpdateTariffPlanAssignment(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new UpdateTariffPlanAssignmentResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("DeleteTariffPlanAssignment")]
        public IActionResult DeleteTariffPlanAssignment([FromBody] DeleteTariffPlanAssignmentRequest request)
        {
            try
            {
                return Ok(_service.DeleteTariffPlanAssignment(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new DeleteTariffPlanAssignmentResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("CopyTariffPlanAssignmentsFromPreviousWeek")]
        public IActionResult CopyTariffPlanAssignmentsFromPreviousWeek([FromBody] CopyTariffPlanAssignmentsFromPreviousWeekRequest request)
        {
            try
            {
                return Ok(_service.CopyTariffPlanAssignmentsFromPreviousWeek(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new CopyTariffPlanAssignmentsFromPreviousWeekResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("GetSettlementItems")]
        public IActionResult GetSettlementItems([FromBody] GetSettlementItemsRequest request)
        {
            try
            {
                return Ok(_service.GetSettlementItems(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetSettlementItemsResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("AddSettlementItem")]
        public IActionResult AddSettlementItem([FromBody] AddSettlementItemRequest request)
        {
            try
            {
                return Ok(_service.AddSettlementItem(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new AddSettlementItemResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("UpdateSettlementItem")]
        public IActionResult UpdateSettlementItem([FromBody] UpdateSettlementItemRequest request)
        {
            try
            {
                return Ok(_service.UpdateSettlementItem(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new UpdateSettlementItemResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("DeleteSettlementItem")]
        public IActionResult DeleteSettlementItem([FromBody] DeleteSettlementItemRequest request)
        {
            try
            {
                return Ok(_service.DeleteSettlementItem(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new DeleteSettlementItemResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("CopySettlementItemsFromPreviousWeek")]
        public IActionResult CopySettlementItemsFromPreviousWeek([FromBody] CopySettlementItemsFromPreviousWeekRequest request)
        {
            try
            {
                return Ok(_service.CopySettlementItemsFromPreviousWeek(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new CopySettlementItemsFromPreviousWeekResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("GetSettlementTypes")]
        public IActionResult GetSettlementTypes([FromBody] GetSettlementTypesRequest request)
        {
            try
            {
                return Ok(_service.GetSettlementTypes(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetSettlementTypesResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("AddSettlementType")]
        public IActionResult AddSettlementType([FromBody] AddSettlementTypeRequest request)
        {
            try
            {
                return Ok(_service.AddSettlementType(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new AddSettlementTypeResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("UpdateSettlementType")]
        public IActionResult UpdateSettlementType([FromBody] UpdateSettlementTypeRequest request)
        {
            try
            {
                return Ok(_service.UpdateSettlementType(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new UpdateSettlementTypeResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("DeleteSettlementType")]
        public IActionResult DeleteSettlementType([FromBody] DeleteSettlementTypeRequest request)
        {
            try
            {
                return Ok(_service.DeleteSettlementType(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new DeleteSettlementTypeResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("ConfirmBillingWeek")]
        public IActionResult ConfirmBillingWeek([FromBody] ConfirmBillingWeekRequest request)
        {
            try
            {
                return Ok(_service.ConfirmBillingWeek(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new ConfirmBillingWeekResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("GetDriverDepositValue")]
        public IActionResult GetDriverDepositValue(GetDriverDepositValueRequest request)
        {
            try
            {
                return Ok(_service.GetDriverDepositValue(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetDriverDepositValueResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }


        [HttpGet("Test")]
        public IActionResult Test()
        {
            return Ok(_service.Test());
        }

        [HttpPost("Ping")]
        public IActionResult Ping([FromBody] string request)
        {
            try
            {
                return Ok(_service.Ping(request));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
