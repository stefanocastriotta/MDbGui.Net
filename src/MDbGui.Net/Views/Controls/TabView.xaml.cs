using GalaSoft.MvvmLight.Messaging;
using MDbGui.Net.Utils;
using MDbGui.Net.ViewModel;
using MDbGui.Net.Views.Dialogs;
using System.Windows;
using System.Windows.Controls;

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
            Messenger.Default.Register<NotificationMessage>(this, (message) => ItemExpandingMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<DocumentResultViewModel>>(this, (message) => EditResultMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<DocumentResultViewModel>>(this, (message) => DeleteResultMessageHandler(message));
        }

        private void ItemExpandingMessageHandler(NotificationMessage message)
        {
            if (message.Notification == Constants.ItemExpandingMessage && message.Target == this.DataContext)
            {
                foreach (var col in grdView.Columns)
                {
                    if (double.IsNaN(col.Width)) col.Width = col.ActualWidth;
                    col.Width = double.NaN;
                }
            }
        }

        private void EditResultMessageHandler(NotificationMessage<DocumentResultViewModel> message)
        {
            if (message.Notification == Constants.EditResultMessage)
            {
                UpdateDocumentView wnd = new UpdateDocumentView();
                var vm = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<ReplaceOneViewModel>();
                vm.Document = message.Content;
                vm.Replacement = message.Content.Result.ToJson(new JsonWriterSettingsExtended() { Indent = true, UseLocalTime = true });
                wnd.DataContext = vm;
                wnd.ShowDialog();
            }
        }

        private void DeleteResultMessageHandler(NotificationMessage<DocumentResultViewModel> message)
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
