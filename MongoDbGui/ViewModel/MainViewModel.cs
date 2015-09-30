using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MongoDbGui.Model;
using System.Collections.ObjectModel;

namespace MongoDbGui.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IMongoDbService _mongoDbService;

        private ObservableCollection<MongoDbServerViewModel> _activeConnections;
        public ObservableCollection<MongoDbServerViewModel> ActiveConnections
        {
            get { return _activeConnections; }
            set
            {
                _activeConnections = value;
                RaisePropertyChanged("ActiveConnections");
            }
        }

        private ObservableCollection<BaseTabViewModel> _tabs;
        public ObservableCollection<BaseTabViewModel> Tabs
        {
            get { return _tabs; }
            set
            {
                _tabs = value;
                RaisePropertyChanged("Tabs");
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IMongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
            _activeConnections = new ObservableCollection<MongoDbServerViewModel>();
            _tabs = new ObservableCollection<BaseTabViewModel>();
            Messenger.Default.Register<NotificationMessage<MongoDbServerViewModel>>(this, (message) => LoginMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<CollectionTabViewModel>>(this, (message) => OpenTabMessageHandler(message));
        }

        private void LoginMessageHandler(NotificationMessage<MongoDbServerViewModel> message)
        {
            if (message.Notification == "LoginSuccessfully")
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ActiveConnections.Add(message.Content);
                });
            }
        }

        private void OpenTabMessageHandler(NotificationMessage<CollectionTabViewModel> message)
        {
            if (message.Notification == "OpenCollectionTab")
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Tabs.Add(message.Content);
                    message.Content.ExecuteFind.Execute(null);
                });
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}