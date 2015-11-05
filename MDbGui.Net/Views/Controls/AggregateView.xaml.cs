﻿using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
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
using MDbGui.Net.Utils;

namespace MDbGui.Net.Views.Controls
{
    /// <summary>
    /// Interaction logic for UpdateView.xaml
    /// </summary>
    public partial class AggregateView : UserControl
    {
        public AggregateView()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage<BsonExtensions.BsonParseException>>(this, (message) => BsonParseExceptionMessageHandler(message));
        }

        private void BsonParseExceptionMessageHandler(NotificationMessage<BsonExtensions.BsonParseException> message)
        {
            if (message.Notification == "AggregateParseException")
            {
                aggregateEditor.CaretOffset = message.Content.Position - 1;
                aggregateEditor.Focus();
            }
        }
    }
}
