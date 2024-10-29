using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class GetDriverDepositValueRequest : BaseRequest
    {
        public required int DriverId { get; set; }
    }
}
