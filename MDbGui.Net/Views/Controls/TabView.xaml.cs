using GalaSoft.MvvmLight.Messaging;
using ICSharpCode.AvalonEdit.Search;
using MDbGui.Net.Utils;
using MDbGui.Net.ViewModel;
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
    /// Interaction logic for TabView.xaml
    /// </summary>
    public partial class TabView : UserControl
    {
        public TabView()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (message) => NotificationMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<DocumentResultViewModel>>(this, (message) => DocumentMessageHandler(message));
        }

        private void NotificationMessageHandler(NotificationMessage message)
        {
            if (message.Notification == Constants.ItemExpandingMessage)
            {
                foreach (var col in grdView.Columns)
                {
                    if (double.IsNaN(col.Width)) col.Width = col.ActualWidth;
                    col.Width = double.NaN;
                }
            }
        }

        private void DocumentMessageHandler(NotificationMessage<DocumentResultViewModel> message)
        {
            if (message.Notification == Constants.ConfirmDeleteResultMessage)
            {
                var result = MessageBox.Show("Delete result with id: " + message.Content.Id + "?", "Delete confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    TabViewModel tabVm = ((ResultsViewModel)message.Content.Parent).Owner;
                    Messenger.Default.Send(new NotificationMessage<DocumentResultViewModel>(this, tabVm, message.Content, Constants.DeleteResultMessage));
                }
            }
        }
    }
}
