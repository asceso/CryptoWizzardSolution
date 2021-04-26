using System;
using System.Globalization;
using System.Windows.Data;

namespace Styles.Converters
{
    public class BooleanRevert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool vb = bool.Parse(value.ToString());
            return !vb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
