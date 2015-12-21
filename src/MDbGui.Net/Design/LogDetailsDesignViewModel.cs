using GalaSoft.MvvmLight;
using System;

namespace MDbGui.Net.Design
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class LogDetailsDesignViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the LogDetailsDesignViewModel class.
        /// </summary>
        public LogDetailsDesignViewModel()
        {
            LogEvent = new log4net.Core.LoggingEvent(new log4net.Core.LoggingEventData() { ExceptionString = "TestException", Level = log4net.Core.Level.Error, Message = "Test error message", TimeStamp = DateTime.Parse("2015/11/09 00:04:00") });
        }

        private log4net.Core.LoggingEvent _logEvent;

        public log4net.Core.LoggingEvent LogEvent
        {
            get
            {
                return _logEvent;
            }
            set
            {
                Set(ref _logEvent, value);
            }
        }

    }
}