using GalaSoft.MvvmLight.Messaging;
using MDbGui.Net.Model;
using MDbGui.Net.ViewModel;
using System.Windows;
using MDbGui.Net.Utils;

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
            Messenger.Default.Register<NotificationMessage<ConnectionInfo>>(this, (message) => LoggingInMessageHandler(message));
        }

        private void LoggingInMessageHandler(NotificationMessage<ConnectionInfo> message)
        {
            if (message.Notification == Constants.LoggingInMessage)
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