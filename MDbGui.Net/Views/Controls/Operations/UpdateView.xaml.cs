using GalaSoft.MvvmLight.Messaging;
using MDbGui.Net.Utils;
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

namespace MDbGui.Net.Views.Controls.Operations
{
    /// <summary>
    /// Interaction logic for UpdateView.xaml
    /// </summary>
    public partial class UpdateView : UserControl
    {
        public UpdateView()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage<BsonExtensions.BsonParseException>>(this, (message) => BsonParseExceptionMessageHandler(message));
        }

        private void BsonParseExceptionMessageHandler(NotificationMessage<BsonExtensions.BsonParseException> message)
        {
            if (message.Notification == "UpdateParseException" && message.Sender == this.DataContext)
            {
                switch (message.Content.PropertyName)
                {
                    case "UpdateFilter":
                        updateFilterEditor.CaretOffset = message.Content.Position;
                        updateFilterEditor.Focus();
                        break;
                    case "UpdateDocument":
                        updateDocumentEditor.CaretOffset = message.Content.Position;
                        updateDocumentEditor.Focus();
                        break;
                }
            }
        }
    }
}
