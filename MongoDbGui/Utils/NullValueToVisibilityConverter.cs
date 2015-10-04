using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MongoDbGui.Utils
{
    public class NullValueToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                if (bool.Parse((string)parameter))
                {
                    return (value != null ? Visibility.Collapsed : Visibility.Visible);
                }
            }
            return (value != null ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; // this shouldn't ever happen, since 
            // you'll need to ensure one-way binding
        }
    }
}
