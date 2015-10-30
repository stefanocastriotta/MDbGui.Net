using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDbGui.Net.Utils
{
    public static class LoggerHelper
    {
        public static ObservableCollection<log4net.Core.LoggingEvent> LogEvents { get; set; }

        public static ILog Logger { get; set; }

        static LoggerHelper()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));

            Logger = LogManager.GetLogger(typeof(String));
            var wpfAppender = Logger.Logger.Repository.GetAppenders().OfType<WpfAppender>().FirstOrDefault();

            if (wpfAppender != null)
            {
                LogEvents = new ObservableCollection<log4net.Core.LoggingEvent>();

                wpfAppender.LogEvents = LogEvents;
            }
        }
    }
}
