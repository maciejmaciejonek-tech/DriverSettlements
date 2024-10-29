using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverSettlements.Common.Models;

namespace DriverSettlements.Common.DataContracts.ApplicationInstance
{
    public class RegisterApplicationInstanceResponse : BaseResponse
    {
        public ApplicationRole ApplicationRole { get; set; } = ApplicationRole.None;
    }
}
