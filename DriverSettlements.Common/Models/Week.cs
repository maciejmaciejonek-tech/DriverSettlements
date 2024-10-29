using DriverSettlements.Common.DataContracts.Tenant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Models
{
    public class Week: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public int YearNumber { get; set; }
        public int WeekNumber { get; set; }
        public DateTime DateFrom { get; set; }  
        public DateTime DateTo { get; set; }
        public BillingWeekStatus BillingWeekStatus { get; set; } = BillingWeekStatus.NotExists;


        public string GetWeekNumber()
        {
            return $"{YearNumber}.{WeekNumber:#00}";
        }

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
