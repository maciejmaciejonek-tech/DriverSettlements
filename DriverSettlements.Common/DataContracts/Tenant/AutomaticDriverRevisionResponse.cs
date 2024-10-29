using DriverSettlements.Common.DataContracts;
using DriverSettlements.Common.Models;
using System.ComponentModel;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class AutomaticDriverRevisionResponse : BaseResponse
    {
        public bool Refresh { get; set; } = false;
    }
}
