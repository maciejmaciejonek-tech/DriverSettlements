using DriverSettlements.Common.DataContracts;
using DriverSettlements.Common.DataContracts.MasterAdmin;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Database;
using DriverSettlements.WebAPI.Configuration;
using DriverSettlements.WebAPI.Controllers.Interfaces;
using DriverSettlements.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TenantSettlements.WebAPI.Services;

namespace DriverSettlements.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterAdminController : ControllerBase, IMasterAdminController
    {
        private readonly MasterAdminService _service;
        private readonly ILogger<MasterAdminController> _logger;


        public MasterAdminController(DatabaseContext dbContext, ILogger<MasterAdminController> logger, ILogger<MasterAdminService> masterAdminServiceLogger, IOptions<ExternalReportsConfiguration> externalReportsConfiguration)
        {
            _logger = logger;
            _service = new MasterAdminService(dbContext, masterAdminServiceLogger, externalReportsConfiguration);
        }

        [HttpPost("GetTenants")]
        public IActionResult GetTenants([FromBody] GetTenantsRequest request)
        {
            try
            {
                return Ok(_service.GetTenants(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetTenantsResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("AddTenant")]
        public IActionResult AddTenant([FromBody] AddTenantRequest request)
        {
            try
            {
                return Ok(_service.AddTenant(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new AddTenantResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("UpdateTenant")]
        public IActionResult UpdateTenant([FromBody] UpdateTenantRequest request)
        {
            try
            {
                return Ok(_service.UpdateTenant(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new UpdateTenantResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("DeleteTenant")]
        public IActionResult DeleteTenant([FromBody] DeleteTenantRequest request)
        {
            try
            {
                return Ok(_service.DeleteTenant(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new DeleteTenantResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("SetTenantInvitationKey")]
        public IActionResult SetTenantInvitationKey([FromBody] SetTenantInvitationKeyRequest request)
        {
            try
            {
                return Ok(_service.SetTenantInvitationKey(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new SetTenantInvitationKeyResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("AddDefaultReportDefinitions")]
        public IActionResult AddDefaultReportDefinitions(AddDefaultReportDefinitionsRequest request)
        {
            try
            {
                return Ok(_service.AddDefaultReportDefinitions(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new AddDefaultReportDefinitionsResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("AddSettlementTypes")]
        public IActionResult AddSettlementTypes(AddSettlementTypesRequest request)
        {
            try
            {
                return Ok(_service.AddSettlementTypes(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new AddSettlementTypesResponse()
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
