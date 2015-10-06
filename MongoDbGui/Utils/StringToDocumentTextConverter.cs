using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MongoDbGui.Utils
{
    public class StringToDocumentTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return new ICSharpCode.AvalonEdit.Document.TextDocument(value.ToString());
            return new ICSharpCode.AvalonEdit.Document.TextDocument();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return ((ICSharpCode.AvalonEdit.Document.TextDocument)value).ToString();
            return null;
        }
    }
}
