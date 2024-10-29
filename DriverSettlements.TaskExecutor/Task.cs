using DriverSettlements.Common.Models;
using DriverSettlements.TaskExecutor.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.TaskExecutor
{
    public class Task
    {
        private readonly ILogger<Task> _logger;

        public Task(ILogger<Task> logger)
        {
            _logger = logger;
        }

        public async Task<bool> Execute(string applicationInstanceId)
        {
            try
            {
                var date = DateTime.Now.AddDays(-7);
                var yearNumber = date.Year;
                var weekNumber = DriverSettlements.Common.Helpers.Date.GetWeekNumber(date);
                var httpClientHandlerService = new HttpsClientHandlerService();
                var tenantService = new TenantService(httpClientHandlerService);
                foreach (var reportType in new[] { ExternalReportType.Bolt, ExternalReportType.Uber, ExternalReportType.Freenow })
                {
                    Console.WriteLine($"Sending request for '{reportType.ToString()}' external report type");
                    try
                    {
                        var response = await tenantService.DownloadExternalReportAsync(new DriverSettlements.Common.DataContracts.Tenant.DownloadExternalReportRequest()
                        {
                            ApplicationInstanceId = applicationInstanceId,
                            YearNumber = yearNumber,
                            WeekNumber = weekNumber,
                            ReportType = reportType,
                            Overwrite = false
                        });
                        if (!response.Result)
                            throw new Exception(response.ErrorMessage);
                        Console.WriteLine($"Report for '{reportType}' type was sucessfully generated");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        Console.WriteLine(ex.Message);
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                throw;
            }
        }
    }
}
