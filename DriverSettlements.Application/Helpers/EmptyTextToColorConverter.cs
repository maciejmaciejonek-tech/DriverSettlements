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
    public class EmptyTextToColorConverter : IValueConverter
    {
        public Color EmptyTextColor { get; set; }
        public Color ValueTextColor { get; set; }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return EmptyTextColor;
            return ValueTextColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
