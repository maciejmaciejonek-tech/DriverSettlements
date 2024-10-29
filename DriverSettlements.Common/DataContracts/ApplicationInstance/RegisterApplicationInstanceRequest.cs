using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.ApplicationInstance
{
    public class RegisterApplicationInstanceRequest : BaseRequest
    {
        public string InvitationKey { get; set; } = string.Empty;
    }
}
