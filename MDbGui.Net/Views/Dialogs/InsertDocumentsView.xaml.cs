using GalaSoft.MvvmLight.Messaging;
using MDbGui.Net.Model;
using System.Windows;

namespace MDbGui.Net.Views.Dialogs
{
    /// <summary>
    /// Description for InsertDocumentsView.
    /// </summary>
    public partial class InsertDocumentsView : Window
    {
        /// <summary>
        /// Initializes a new instance of the InsertDocumentsView class.
        /// </summary>
        public InsertDocumentsView()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage<InsertDocumentsModel>>(this, (message) => NotificationMessageHandler(message));
        }

        private void NotificationMessageHandler(NotificationMessage<InsertDocumentsModel> message)
        {
            if (message.Notification == "InsertDocuments")
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