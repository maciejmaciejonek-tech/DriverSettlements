using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.ApplicationInstance
{
    public class GetApplicationInstanceRoleResponse : BaseResponse
    {
        public ApplicationRole ApplicationRole { get; set; } = ApplicationRole.None;
    }
}
