using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class GetTariffPlanAssignmentsResponse : BaseResponse
    {
        public List<TariffPlanAssignment> TariffPlanAssignments { get; set; } = new List<TariffPlanAssignment>();
        public BillingWeekStatus BillingWeekStatus { get; set; } = BillingWeekStatus.NotExists;
    }
}
