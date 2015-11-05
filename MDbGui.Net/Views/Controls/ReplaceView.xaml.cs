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
    /// Interaction logic for ReplaceView.xaml
    /// </summary>
    public partial class ReplaceView : UserControl
    {
        public ReplaceView()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage<BsonExtensions.BsonParseException>>(this, (message) => BsonParseExceptionMessageHandler(message));
        }

        private void BsonParseExceptionMessageHandler(NotificationMessage<BsonExtensions.BsonParseException> message)
        {
            if (message.Notification == "ReplaceParseException" && message.Sender == this.DataContext)
            {
                switch (message.Content.PropertyName)
                {
                    case "ReplaceFilter":
                        replaceFilterEditor.CaretOffset = message.Content.Position;
                        replaceFilterEditor.Focus();
                        break;
                    case "Replacement":
                        replacementEditor.CaretOffset = message.Content.Position;
                        replacementEditor.Focus();
                        break;
                }
            }
        }
    }
}
