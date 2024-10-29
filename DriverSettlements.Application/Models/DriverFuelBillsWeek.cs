using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.App.Models
{
    public class DriverFuelBillsWeek : Week
    {
        public List<FuelBill> FuelBills { get;set; } 
    }
}
