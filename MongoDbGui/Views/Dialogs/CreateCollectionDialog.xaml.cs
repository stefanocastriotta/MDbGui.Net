using GalaSoft.MvvmLight.Messaging;
using MongoDbGui.ViewModel;
using System.Windows;

namespace MongoDbGui.Views.Dialogs
{
    /// <summary>
    /// Description for CreateCollectionDialog.
    /// </summary>
    public partial class CreateCollectionDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the CreateCollectionDialog class.
        /// </summary>
        public CreateCollectionDialog()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage<CreateCollectionViewModel>>(this, (message) => NotificationMessageHandler(message));
        }

        private void NotificationMessageHandler(NotificationMessage<CreateCollectionViewModel> message)
        {
            if (message.Notification == "CreateCollection")
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