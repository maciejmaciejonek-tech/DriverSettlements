using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.App.Helpers
{
    public class ColorHelper
    {
        public static Color GetTitleGridBacgroundColor(Week week)
        {
            return week!=null && week.BillingWeekStatus==BillingWeekStatus.Closed  ? Colors.LightGreen : Colors.LemonChiffon;
        }
    }
}
