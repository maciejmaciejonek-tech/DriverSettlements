using DriverSettlements.Common.DataContracts.ApplicationInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Interfaces
{
    public interface IApplicationInstanceAsync
    {
        Task<RegisterApplicationInstanceResponse> RegisterApplicationInstanceAsync(RegisterApplicationInstanceRequest request);
        Task<GetApplicationInstanceRoleResponse> GetApplicationInstanceRoleAsync(GetApplicationInstanceRoleRequest request);
        Task<string> PingAsync(string ping);
    }
}
