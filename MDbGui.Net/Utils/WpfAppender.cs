using GalaSoft.MvvmLight.Threading;
using log4net.Appender;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDbGui.Net.Utils
{
    public class WpfAppender : AppenderSkeleton
    {
        public ObservableCollection<log4net.Core.LoggingEvent> LogEvents { get; set; }

        /// <summary>
        /// Addes a log entry to the Execution Control's LogEntries property
        /// http://www.dotmaniac.net/display-log4net-entries-in-wpf-datagrid-in-real-time/
        /// </summary>
        /// <remarks>
        /// Appender does not log debug messages
        /// </remarks>
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                LogEvents.Add(loggingEvent);
            });
        }

    }
}
