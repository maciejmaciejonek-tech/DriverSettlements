using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts
{
    public abstract class BaseRequest
    {
        public required string ApplicationInstanceId { get; set; }
    }
}
