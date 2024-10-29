using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class GetSettlementItemsResponse : BaseResponse
    {
        public List<SettlementItem> SettlementItems { get; set; } = new List<SettlementItem>();
        public BillingWeekStatus BillingWeekStatus { get; set; } = BillingWeekStatus.NotExists;
    }
}
