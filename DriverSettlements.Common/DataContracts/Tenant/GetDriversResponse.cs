using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class GetDriversResponse : BaseResponse
    {
        public List<Driver> Drivers { get; set; } = new List<Driver>();
    }
}
