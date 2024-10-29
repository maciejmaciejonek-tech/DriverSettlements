using DriverSettlements.Common.Models;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class GetBasicReportResponse : BaseResponse
    {
        public Report? Report { get; set; }
    }
}