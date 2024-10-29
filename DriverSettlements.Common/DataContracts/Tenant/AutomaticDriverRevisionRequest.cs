using DriverSettlements.Common.DataContracts;
using DriverSettlements.Common.Models;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class AutomaticDriverRevisionRequest : BaseRequest
    {
        public int YearNumber { get; set; }
        public int WeekNumber { get; set; }
    }
}
