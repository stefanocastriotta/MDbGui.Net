﻿using GalaSoft.MvvmLight.Messaging;
using log4net.Core;
using MDbGui.Net.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MDbGui.Net.Views.Controls
{
    /// <summary>
    /// Logica di interazione per LogView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        public LogView()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)grdLogs.Items).CollectionChanged += LogView_CollectionChanged;
            Messenger.Default.Register<NotificationMessage<log4net.Core.LoggingEvent>>(this, (message) => LogDetailsMessageHandler(message));
        }

        private void LogView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (grdLogs.Items.Count > 0)
                grdLogs.ScrollIntoView(grdLogs.Items[grdLogs.Items.Count - 1]);
        }

        private void LogDetailsMessageHandler(NotificationMessage<LoggingEvent> message)
        {
            if (message.Notification == Constants.ShowLogDetailsMessage)
            {
                LogDetailsView logDetails = new LogDetailsView();
                logDetails.DataContext = message.Content;
                logDetails.ShowDialog();
            }
        }
    }
}
