using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.App.Models
{
    public class TenantFuelBillsWeek : Week
    {
        public List<DriverFuelBillsInfo> DriverFuelBillsInfos { get; set; }
    }
}
