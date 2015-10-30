using System.Windows;
using MDbGui.Net.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using MDbGui.Net.Views.Dialogs;
using MDbGui.Net.Views.Controls;
using MongoDB.Bson;
using MDbGui.Net.Utils;

namespace MDbGui.Net.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            Messenger.Default.Register<NotificationMessage<MongoDbCollectionViewModel>>(this, (message) => CollectionMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<MongoDbDatabaseViewModel>>(this, (message) => DatabaseMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<DocumentResultViewModel>>(this, (message) => DocumentMessageHandler(message));
            Utils.LoggerHelper.Logger.Debug("Application started");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoginView wnd = new LoginView();
            wnd.ShowDialog();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginView wnd = new LoginView();
            wnd.ShowDialog();
        }

        private void CollectionMessageHandler(NotificationMessage<MongoDbCollectionViewModel> message)
        {
            if (message.Notification == "ConfirmDropCollection")
            {
                var result = MessageBox.Show("Drop collection " + message.Content.Name + "?", "Drop confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Messenger.Default.Send(new NotificationMessage<MongoDbCollectionViewModel>(this, message.Content.Database, message.Content, "DropCollection"));
                }
            }
        }

        private void DatabaseMessageHandler(NotificationMessage<MongoDbDatabaseViewModel> message)
        {
            switch (message.Notification)
            {
                case "OpenCreateNewCollection":
                    CreateCollectionDialog wnd = new CreateCollectionDialog();
                    var vm = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<CreateCollectionViewModel>();
                    vm.Database = message.Content;
                    wnd.DataContext = vm;
                    wnd.ShowDialog();
                    break;
                case "ConfirmDropDatabase":
                    var result = MessageBox.Show("Drop database " + message.Content.Name + "?", "Drop confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Messenger.Default.Send(new NotificationMessage<MongoDbDatabaseViewModel>(this, message.Content.Server, message.Content, "DropDatabase"));
                    }
                    break;
            }
        }

        private void DocumentMessageHandler(NotificationMessage<DocumentResultViewModel> message)
        {
            if (message.Notification == "EditResult")
            {
                UpdateDocumentView wnd = new UpdateDocumentView();
                var vm = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<ReplaceOneViewModel>();
                vm.Document = message.Content;
                vm.Replacement = message.Content.Result.ToJson(new JsonWriterSettingsExtended() { Indent = true, UseLocalTime = true });
                wnd.DataContext = vm;
                wnd.ShowDialog();
            }
        }
    }
}