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
    }
}