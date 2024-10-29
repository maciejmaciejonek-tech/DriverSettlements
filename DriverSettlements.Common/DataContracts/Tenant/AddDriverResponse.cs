using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class AddDriverResponse : BaseResponse
    {
        public int? DriverId { get; set; } = null;
    }
}
