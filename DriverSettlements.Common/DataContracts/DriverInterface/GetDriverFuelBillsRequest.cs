using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.DriverInterface
{
    public class GetDriverFuelBillsRequest : BaseRequest
    {
        public required int YearNumber { get; set; }
        public required int WeekNumber { get; set; }
    }
}
