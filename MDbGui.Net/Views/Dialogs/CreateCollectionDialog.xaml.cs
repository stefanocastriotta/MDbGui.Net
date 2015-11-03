using GalaSoft.MvvmLight.Messaging;
using MDbGui.Net.ViewModel;
using System.Windows;

namespace MDbGui.Net.Views.Dialogs
{
    /// <summary>
    /// Description for CreateCollectionDialog.
    /// </summary>
    public partial class CreateCollectionDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the CreateCollectionDialog class.
        /// </summary>
        public CreateCollectionDialog()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage<CreateCollectionViewModel>>(this, (message) => NotificationMessageHandler(message));
            if (GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
            {
                var vmTest = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<CreateCollectionViewModel>();
                vmTest.Name = "Collection1";
                vmTest.StorageEngine = "{ \"Property1\": \"Value1\"}";
                this.DataContext = vmTest;
            }
        }

        private void NotificationMessageHandler(NotificationMessage<CreateCollectionViewModel> message)
        {
            if (message.Notification == "CreateCollection")
            {
                this.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}