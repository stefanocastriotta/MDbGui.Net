using System.Windows;
using MongoDbGui.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using MongoDbGui.Views.Dialogs;

namespace MongoDbGui.Views
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
            Messenger.Default.Register<NotificationMessage<MongoDbDatabaseViewModel>>(this, (message) => OpenCreateNewCollectionMessageHandler(message));
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
            if (message.Notification == "OpenInsertDocuments")
            {
                InsertDocumentsView wnd = new InsertDocumentsView();
                var vm = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<InsertDocumentsViewModel>();
                vm.Collection = message.Content;
                wnd.DataContext = vm;
                wnd.ShowDialog();
            }
            else if (message.Notification == "ConfirmDropCollection")
            {
                var result = MessageBox.Show("Drop collection " + message.Content.Name + "?", "Drop confirm", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Messenger.Default.Send(new NotificationMessage<MongoDbCollectionViewModel>(message.Content, "DropCollection"));
                }
            }
        }

        private void OpenCreateNewCollectionMessageHandler(NotificationMessage<MongoDbDatabaseViewModel> message)
        {
            if (message.Notification == "OpenCreateNewCollection")
            {
                CreateCollectionDialog wnd = new CreateCollectionDialog();
                var vm = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<CreateCollectionViewModel>();
                vm.Database = message.Content;
                wnd.DataContext = vm;
                wnd.ShowDialog();
            }
        }
    }
}