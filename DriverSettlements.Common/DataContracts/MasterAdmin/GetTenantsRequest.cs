using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.MasterAdmin
{
    public class GetTenantsRequest : BaseRequest
    {
        public bool IncludeDeleted { get; set; } = false;
    }
}
