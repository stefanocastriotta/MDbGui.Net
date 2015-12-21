using log4net.Core;
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
            if (item != null)
            {
                var logItem = (LoggingEvent)item;

                if (logItem.Level == Level.Error)
                        return ErrorTemplate;
                else if (logItem.Level == Level.Warn)
                    return WarningTemplate;
                else if (logItem.Level == Level.Info)
                    return InfoTemplate;
                else if (logItem.Level == Level.Debug)
                    return DebugTemplate;
            }
            return null;
        }
    }

}
