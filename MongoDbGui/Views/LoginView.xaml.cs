using GalaSoft.MvvmLight.Messaging;
using MongoDbGui.ViewModel;
using System.Windows;

namespace MongoDbGui.Views
{
    /// <summary>
    /// Description for LoginView.
    /// </summary>
    public partial class LoginView : Window
    {
        /// <summary>
        /// Initializes a new instance of the LoginView class.
        /// </summary>
        public LoginView()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage<MongoDbServerViewModel>>(this, (message) => NotificationMessageHandler(message));
        }

        private void NotificationMessageHandler(NotificationMessage<MongoDbServerViewModel> message)
        {
            if (message.Notification == "LoginSuccessfully")
            {
                this.Close();
            }
        }
    }
}