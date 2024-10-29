using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.App.Models
{
    public class TenantSettlementItemsWeek : Week
    {
        public List<SettlementItem> SettlementItems;
    }
}
