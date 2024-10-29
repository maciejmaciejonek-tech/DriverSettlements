using DriverSettlements.App.Controllers;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.App.Helpers
{
    public class SelectedItemBackgroudColorConverter : IValueConverter
    {
        public Color SelectedItemColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? SelectedItemColor : Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
