using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class WeeklySettlementsInfo
    {
        public int NumberOfTariffPlanAssignments { get; set; }
        public int NumberOfRequestedFuelBills { get; set; }
        public int NumberOfAcceptedFuelBills { get; set; }
        public int NumberOfRejectedFuelBills { get; set; }
        public int NumberOfSettlementItems { get; set; }
    }
}
