using System.Windows;
using MongoDbGui.ViewModel;
using GalaSoft.MvvmLight.Messaging;

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
            Messenger.Default.Register<string>(this, (message) => StringMessageActionHandler(message));
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void StringMessageActionHandler(string message)
        {
            if (message == "ShowLoginDialog")
            {
                LoginView wnd = new LoginView();
                wnd.ShowDialog();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send<string>("ShowLoginDialog");
        }
    }
}