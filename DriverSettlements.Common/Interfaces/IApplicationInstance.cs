using DriverSettlements.Common.DataContracts.ApplicationInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Interfaces
{
    public interface IApplicationInstance
    {

        RegisterApplicationInstanceResponse RegisterApplicationInstance(RegisterApplicationInstanceRequest request);
        GetApplicationInstanceRoleResponse GetApplicationInstanceRole(GetApplicationInstanceRoleRequest request);
        string Ping(string ping);
        string Test();

    }
}
