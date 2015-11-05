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

namespace MDbGui.Net.Views.Controls
{
    /// <summary>
    /// Interaction logic for UpdateView.xaml
    /// </summary>
    public partial class RemoveView : UserControl
    {
        public RemoveView()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage<BsonExtensions.BsonParseException>>(this, (message) => BsonParseExceptionMessageHandler(message));
        }

        private void BsonParseExceptionMessageHandler(NotificationMessage<BsonExtensions.BsonParseException> message)
        {
            if (message.Notification == "DeleteParseException" && message.Sender == this.DataContext && message.Content.PropertyName == "DeleteQuery")
            {
                removeEditor.CaretOffset = message.Content.Position;
                removeEditor.Focus();
            }
        }
    }
}
