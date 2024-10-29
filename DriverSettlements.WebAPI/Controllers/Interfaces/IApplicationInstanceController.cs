using DriverSettlements.Common.DataContracts.ApplicationInstance;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.WebAPI.Controllers.Interfaces
{
    public interface IApplicationInstanceController
    {
        IActionResult RegisterApplicationInstance(RegisterApplicationInstanceRequest request);
        IActionResult GetApplicationInstanceRole(GetApplicationInstanceRoleRequest request);
        IActionResult Ping(string ping);
        IActionResult Test();
    }
}
