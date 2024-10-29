using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class AddSettlementTypeResponse : BaseResponse
    {
        public int? SettlementTypeId { get; set; } = null;
    }
}
