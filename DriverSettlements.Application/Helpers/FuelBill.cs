using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.App.Helpers
{
    public class FuelBill
    {
        public static string StatusToText(FuelBillStatus fuelBillStatus)
        {
            switch (fuelBillStatus)
            {
                case FuelBillStatus.Acccepted:
                    return "Zatwierdzony";
                case FuelBillStatus.Rejected:
                    return "Odrzucony";
                case FuelBillStatus.Requested:
                    return "Oczekujący";
                case FuelBillStatus.Deleted:
                    return "Usunięty";
                default:
                    return "Nieznany";
            }
        }

        public static Color StatusToColor(FuelBillStatus fuelBillStatus)
        {
            switch (fuelBillStatus)
            {
                case FuelBillStatus.Acccepted:
                    return Colors.LightGreen;
                case FuelBillStatus.Rejected:
                    return Colors.OrangeRed;
                case FuelBillStatus.Requested:
                    return Colors.LightGoldenrodYellow;
                case FuelBillStatus.Deleted:
                    return Colors.LightGray;
                default:
                    return Colors.LightGray;
            }
        }

    }
}
