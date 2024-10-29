using DriverSettlements.Common.Models;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class GetCustomReportResponse : BaseResponse
    {
        public Report? Report { get; set; }
    }
}