using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace EsmoChamps.Converters
{
    public class RangeToGridLengthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double min = System.Convert.ToDouble(values[0]);
            double max = System.Convert.ToDouble(values[1]);
            string type = parameter.ToString();

            return type switch
            {
                "Pre" => new GridLength(min, GridUnitType.Star),
                "Fill" => new GridLength(max - min, GridUnitType.Star),
                "Post" => new GridLength(100 - max, GridUnitType.Star),
                _ => new GridLength(0)
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
