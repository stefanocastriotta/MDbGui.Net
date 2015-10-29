﻿using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using MDbGui.Net.Model;
using MDbGui.Net.ViewModel;
using System.Windows;

namespace MDbGui.Net.Views.Dialogs
{
    /// <summary>
    /// Description for LoginView.
    /// </summary>
    public partial class LoginView : Window
    {
        LoginViewModel vm;

        /// <summary>
        /// Initializes a new instance of the LoginView class.
        /// </summary>
        public LoginView()
        {
            InitializeComponent();
            vm = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<LoginViewModel>();
            this.DataContext = vm;
            Closing += (s, e) => vm.Cleanup();
            Messenger.Default.Register<NotificationMessage<ConnectionInfo>>(this, (message) => NotificationMessageHandler(message));
        }

        private void NotificationMessageHandler(NotificationMessage<ConnectionInfo> message)
        {
            if (message.Notification == "LoggingIn")
            {
                this.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}