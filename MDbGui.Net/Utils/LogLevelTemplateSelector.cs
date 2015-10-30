using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MDbGui.Net.Utils
{
    public class LogLevelTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ErrorTemplate { get; set; }

        public DataTemplate WarningTemplate { get; set; }

        public DataTemplate InfoTemplate { get; set; }

        public DataTemplate DebugTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            string level = item.ToString();

            switch (level)
            {
                case "Error":
                    return ErrorTemplate;
                case "Warning":
                    return WarningTemplate;
                case "Info":
                    return InfoTemplate;
                default:
                    return DebugTemplate;
            }
        }
    }

}
