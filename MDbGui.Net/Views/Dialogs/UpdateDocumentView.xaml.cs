using GalaSoft.MvvmLight.Messaging;
using MDbGui.Net.ViewModel;
using System.Windows;

namespace MDbGui.Net.Views.Dialogs
{
    /// <summary>
    /// Description for InsertDocumentsView.
    /// </summary>
    public partial class UpdateDocumentView : Window
    {
        /// <summary>
        /// Initializes a new instance of the InsertDocumentsView class.
        /// </summary>
        public UpdateDocumentView()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage<ReplaceOneViewModel>>(this, (message) => NotificationMessageHandler(message));
            Closing += (s, e) =>
            {
                ((ReplaceOneViewModel)this.DataContext).Cleanup();
            };
        }

        private void NotificationMessageHandler(NotificationMessage<ReplaceOneViewModel> message)
        {
            if (message.Notification == "UpdateDocument")
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