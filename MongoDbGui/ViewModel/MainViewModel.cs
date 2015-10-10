using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDbGui.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

        private BaseTabViewModel _selectedTab;

        public BaseTabViewModel SelectedTab
        {
            get
            {
                return _selectedTab;
            }
            set
            {
                Set(ref _selectedTab, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _activeConnections = new ObservableCollection<MongoDbServerViewModel>();
            _tabs = new ObservableCollection<BaseTabViewModel>();
            Messenger.Default.Register<NotificationMessage<ConnectionInfo>>(this, (message) => LoggingInMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<CollectionTabViewModel>>(this, (message) => TabMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<MongoDbServerViewModel>>(this, (message) => MongoDbServerMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<InsertDocumentsModel>>(this, (message) => InsertDocumentsMessageHandler(message));
        }

        private async void LoggingInMessageHandler(NotificationMessage<ConnectionInfo> message)
        {
            if (message.Notification == "LoggingIn")
            {
                var _mongoDbService = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<IMongoDbService>();
                MongoDbServerViewModel serverVm = new MongoDbServerViewModel(_mongoDbService);
                serverVm.IsBusy = true;
                serverVm.Name = message.Content.Address + ":" + message.Content.Port;

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ActiveConnections.Add(serverVm);
                });

                try
                {
                    var serverInfo = await _mongoDbService.Connect(message.Content);
                    List<MongoDbDatabaseViewModel> systemDatabases = new List<MongoDbDatabaseViewModel>();
                    List<MongoDbDatabaseViewModel> standardDatabases = new List<MongoDbDatabaseViewModel>();

                    FolderViewModel systemDbFolder = new FolderViewModel("System", serverVm);
                    foreach (var database in serverInfo.Databases)
                    {
                        var databaseVm = new MongoDbDatabaseViewModel(serverVm, database["name"].AsString);
                        databaseVm.SizeOnDisk = database["sizeOnDisk"].AsDouble;
                        if (databaseVm.Name == "local")
                            systemDatabases.Add(databaseVm);
                        else
                            standardDatabases.Add(databaseVm);
                    }

                    foreach (var systemDb in systemDatabases.OrderBy(o => o.Name))
                        systemDbFolder.Children.Add(systemDb);

                    serverVm.Items.Add(systemDbFolder);

                    foreach (var db in standardDatabases.OrderBy(o => o.Name))
                        serverVm.Items.Add(db);

                    serverVm.IsExpanded = true;
                }
                catch (Exception ex)
                {

                }
                serverVm.IsBusy = false;
            }
        }


        private void TabMessageHandler(NotificationMessage<CollectionTabViewModel> message)
        {
            switch (message.Notification)
            {
                case "OpenCollectionTab":
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Tabs.Add(message.Content);
                    SelectedTab = message.Content;
                    message.Content.ExecuteFind.Execute(null);
                });
                break;
                case "CloseTab":
                Tabs.Remove(message.Content);
                break;
            }
        }

        private void MongoDbServerMessageHandler(NotificationMessage<MongoDbServerViewModel> message)
        {
            if (message.Notification == "Disconnect")
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ActiveConnections.Remove(message.Content);
                });
            }
        }

        private async void InsertDocumentsMessageHandler(NotificationMessage<InsertDocumentsModel> message)
        {
            if (message.Notification == "InsertDocuments")
            {
                message.Content.Collection.IsBusy = true;
                BsonArray array = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(message.Content.Documents);
                await message.Content.Collection.Database.Server.MongoDbService.Insert(message.Content.Collection.Database.Name, message.Content.Collection.Name, array.Select(i => i.AsBsonDocument));
                message.Content.Collection.IsBusy = false;
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}