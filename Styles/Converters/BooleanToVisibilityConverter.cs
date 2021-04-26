using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Styles.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool vb = bool.Parse(value.ToString());
            bool pb = bool.Parse(parameter.ToString());
            if (vb == pb) return Visibility.Visible;
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
