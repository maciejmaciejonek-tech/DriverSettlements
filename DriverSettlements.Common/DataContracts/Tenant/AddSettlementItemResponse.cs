using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class AddSettlementItemResponse : BaseResponse
    {
        public int? SettlementItemId { get; set; } = null;
    }
}
