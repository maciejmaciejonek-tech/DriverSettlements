using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class GetTariffPlansResponse : BaseResponse
    {
        public List<TariffPlan> TariffPlans { get; set; } = new List<TariffPlan>();
    }
}
