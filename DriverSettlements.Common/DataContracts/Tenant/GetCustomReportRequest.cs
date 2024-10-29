using DriverSettlements.Common.Models;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class GetCustomReportRequest : BaseRequest
    {
        public List<Week> Weeks { get; set; } = new List<Week>();
    }
}