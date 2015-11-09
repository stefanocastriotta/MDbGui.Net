using GalaSoft.MvvmLight.Messaging;
using MDbGui.Net.Utils;
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
            Messenger.Default.Register<NotificationMessage<BsonExtensions.BsonParseException>>(this, (message) => BsonParseExceptionMessageHandler(message));
        }

        private void NotificationMessageHandler(NotificationMessage<ReplaceOneViewModel> message)
        {
            if (message.Notification == "UpdateDocument")
            {
                this.Close();
            }
        }

        private void BsonParseExceptionMessageHandler(NotificationMessage<BsonExtensions.BsonParseException> message)
        {
            if (message.Notification == Constants.ReplaceOneParseException && message.Sender == this.DataContext && message.Content.PropertyName == Constants.ReplacementProperty)
            {
                replaceDocumentEditor.CaretOffset = message.Content.Position;
                replaceDocumentEditor.Focus();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}