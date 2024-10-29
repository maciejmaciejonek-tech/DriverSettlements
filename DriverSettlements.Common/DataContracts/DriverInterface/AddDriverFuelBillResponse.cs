using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.DriverInterface
{
    public class AddDriverFuelBillResponse: BaseResponse
    {
        public int? FuelBillId { get; set; } = null;
    }
}
