using DriverSettlements.Common;
using DriverSettlements.Common.DataContracts;
using DriverSettlements.Common.DataContracts.ApplicationInstance;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Database;
using DriverSettlements.WebAPI.Controllers.Interfaces;
using DriverSettlements.WebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DriverSettlements.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationInstanceController : ControllerBase, IApplicationInstanceController
    {
        private readonly ApplicationInstanceService _service;
        private readonly ILogger<ApplicationInstanceController> _logger;


        public ApplicationInstanceController(DatabaseContext dbContext, ILogger<ApplicationInstanceController> logger, ILogger<ApplicationInstanceService> applicationInstanceServiceLogger)
        {
            _logger = logger;   
            _service = new ApplicationInstanceService(dbContext, applicationInstanceServiceLogger);
        }

        [HttpPost("RegisterApplicationInstance")]
        public IActionResult RegisterApplicationInstance([FromBody] RegisterApplicationInstanceRequest request)
        {
            try
            {
                return Ok(_service.RegisterApplicationInstance(request));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new RegisterApplicationInstanceResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("GetApplicationInstanceRole")]
        public IActionResult GetApplicationInstanceRole([FromBody] GetApplicationInstanceRoleRequest request)
        {
            try
            {
                return Ok(_service.GetApplicationInstanceRole(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetApplicationInstanceRoleResponse()
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
