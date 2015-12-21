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
            if (message.Notification == Constants.FindParseException && message.Sender == this.DataContext)
            {
                switch (message.Content.PropertyName)
                {
                    case Constants.FindProperty:
                        findEditor.CaretOffset = message.Content.Position;
                        findEditor.Focus();
                        break;
                    case Constants.SortProperty:
                        sortEditor.CaretOffset = message.Content.Position;
                        sortEditor.Focus();
                        break;
                    case Constants.ProjectionProperty:
                        projectionEditor.CaretOffset = message.Content.Position;
                        projectionEditor.Focus();
                        break;
                }
            }
        }
    }
}
