using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.Common.Models
{
    public class DriverWeek: Week
    {
        private BillingWeek _billingWeek;

        public BillingWeek BillingWeek
        { 
            get => _billingWeek;
            set
            {
                _billingWeek = value;
                OnPropertyChanged("BillingWeek");
            }
        }
    }
}
