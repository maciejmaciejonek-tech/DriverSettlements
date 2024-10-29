using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class GetSettlementTypesResponse : BaseResponse
    {
        public List<SettlementType> SettlementTypes { get; set; } = new List<SettlementType>();
    }
}
