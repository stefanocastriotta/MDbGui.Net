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
    /// Interaction logic for FindView.xaml
    /// </summary>
    public partial class FindView : UserControl
    {
        public FindView()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage<BsonExtensions.BsonParseException>>(this, (message) => BsonParseExceptionMessageHandler(message));
        }

        private void BsonParseExceptionMessageHandler(NotificationMessage<BsonExtensions.BsonParseException> message)
        {
            if (message.Notification == "FindParseException" && message.Sender == this.DataContext)
            {
                switch (message.Content.PropertyName)
                {
                    case "Find":
                        findEditor.CaretOffset = message.Content.Position;
                        findEditor.Focus();
                        break;
                    case "Sort":
                        sortEditor.CaretOffset = message.Content.Position;
                        sortEditor.Focus();
                        break;
                    case "Projection":
                        projectionEditor.CaretOffset = message.Content.Position;
                        projectionEditor.Focus();
                        break;
                }
            }
        }
    }
}
