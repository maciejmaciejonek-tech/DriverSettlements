using DriverSettlements.Common.DataContracts.DriverInterface;
using DriverSettlements.Common.DataContracts.Tenant;
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
    public class DriverController : ControllerBase, IDriverController
    {
        private readonly DriverService _service;
        private readonly ILogger<DriverController> _logger;


        public DriverController(DatabaseContext dbContext, ILogger<DriverController> logger, ILogger<DriverService> driverServiceLogger, IOptions<PhotosConfiguration> photosConfiguration)
        {
            _logger = logger;
            _service = new DriverService(dbContext, driverServiceLogger, photosConfiguration);
        }


        [HttpPost("GetBillingWeek")]
        public IActionResult GetBillingWeek([FromBody] GetBillingWeekRequest request)
        {
            try
            {
                return Ok(_service.GetBillingWeek(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetBillingWeekResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("GetDriverFuelBills")]
        public IActionResult GetDriverFuelBills([FromBody] GetDriverFuelBillsRequest request)
        {
            try
            {
                return Ok(_service.GetDriverFuelBills(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new GetDriverFuelBillsResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("AddDriverFuelBill")]
        public IActionResult AddDriverFuelBill([FromBody] AddDriverFuelBillRequest request)
        {
            try
            {
                return Ok(_service.AddDriverFuelBill(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new AddDriverFuelBillResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("UpdateDriverFuelBill")]
        public IActionResult UpdateDriverFuelBill([FromBody] UpdateDriverFuelBillRequest request)
        {
            try
            {
                return Ok(_service.UpdateDriverFuelBill(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new UpdateDriverFuelBillResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("DeleteDriverFuelBill")]
        public IActionResult DeleteDriverFuelBill([FromBody] DeleteDriverFuelBillRequest request)
        {
            try
            {
                return Ok(_service.DeleteDriverFuelBill(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new DeleteDriverFuelBillResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("UploadDriverFuelBillPhoto")]
        public IActionResult UploadDriverFuelBillPhoto([FromBody] UploadDriverFuelBillPhotoRequest request)
        {
            try
            {
                return Ok(_service.UploadDriverFuelBillPhoto(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new UploadDriverFuelBillPhotoResponse()
                {
                    Result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("DownloadDriverFuelBillPhoto")]
        public IActionResult DownloadDriverFuelBillPhoto(DownloadDriverFuelBillPhotoRequest request)
        {
            try
            {
                return Ok(_service.DownloadDriverFuelBillPhoto(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new DownloadDriverFuelBillPhotoResponse()
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
