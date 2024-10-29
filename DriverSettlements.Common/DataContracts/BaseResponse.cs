using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts
{
    public abstract class BaseResponse
    {
        public bool Result { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;

    }
}
