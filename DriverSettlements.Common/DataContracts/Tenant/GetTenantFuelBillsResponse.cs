using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class GetTenantFuelBillsResponse : BaseResponse
    {
        public List<FuelBill>? FuelBills { get; set; }
        public BillingWeekStatus BillingWeekStatus { get; set; } = BillingWeekStatus.NotExists;
    }
}
