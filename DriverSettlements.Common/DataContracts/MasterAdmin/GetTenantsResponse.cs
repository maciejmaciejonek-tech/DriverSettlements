using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.MasterAdmin
{
    public class GetTenantsResponse : BaseResponse
    {
        public List<LegalEntity> Tenants { get; set; } = new List<LegalEntity>();
    }
}
