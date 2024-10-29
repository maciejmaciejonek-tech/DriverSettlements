using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.DataContracts.Tenant
{
    public class DriverFuelBillsInfo
    {
        public required int DriverId { get; set; }
        public required string DriverName { get; set; }
        public required int Requested { get; set; }
        public required int Accepted { get; set; }
        public required int Rejected { get; set; }

        public List<FuelBill>? FuelBils { get; set; }
    }
}
