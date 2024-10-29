using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class GetTariffPlansRequest : BaseRequest
    {
        public bool IncludeDeleted { get; set; } = false;
    }
}
