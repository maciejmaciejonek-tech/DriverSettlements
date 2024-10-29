using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Models
{
    public enum ApplicationRole
    {
        None = 0,
        MasterAdmin = 1,
        Tenant = 2,
        Driver = 3,
        Blocked = 9
    }
}
