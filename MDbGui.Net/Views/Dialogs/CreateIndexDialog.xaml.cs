using GalaSoft.MvvmLight.Messaging;
using MDbGui.Net.Utils;
using MDbGui.Net.ViewModel;
using System.Windows;

namespace MDbGui.Net.Views.Dialogs
{
    /// <summary>
    /// Logica di interazione per CreateIndexView.xaml
    /// </summary>
    public partial class CreateIndexDialog : Window
    {
        public CreateIndexDialog()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage<CreateIndexViewModel>>(this, (message) => CreateIndexMessageHandler(message));
        }

        private void CreateIndexMessageHandler(NotificationMessage<CreateIndexViewModel> message)
        {
            if (message.Notification == Constants.CreateIndexMessage || message.Notification == Constants.RecreateIndexMessage)
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
