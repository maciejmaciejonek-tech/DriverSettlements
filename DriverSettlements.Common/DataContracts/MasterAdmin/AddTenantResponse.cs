using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.MasterAdmin
{
    public class AddTenantResponse : BaseResponse
    {
        public int? TenantId { get; set; } = null;
    }
}
