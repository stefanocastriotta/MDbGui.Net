using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MDbGui.Net.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using System;
using System.Threading;
using MongoDB.Driver.Core.Misc;
using GalaSoft.MvvmLight.Ioc;

namespace MDbGui.Net.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MongoDbDatabaseViewModel : BaseTreeviewViewModel, IDisposable
    {
        public Dictionary<string, DatabaseCommand> DatabaseCommands { get; set; }

        CancellationTokenSource cts = new CancellationTokenSource();

        private bool _disposed;

        private MongoDbServerViewModel _server;
        public MongoDbServerViewModel Server
        {
            get { return _server; }
            set
            {
                Set(ref _server, value);
            }
        }

        private bool _collectionsLoaded;

        private bool _usersLoaded;

        private ObservableCollection<FolderViewModel> _folders;
        public ObservableCollection<FolderViewModel> Folders
        {
            get { return _folders; }
            set
            {
                _folders = value;
                RaisePropertyChanged("Folders");
            }
        }

        private FolderViewModel _collections;
        private FolderViewModel _users;

        /// <summary>
        /// Initializes a new instance of the MongoDbDatabaseViewModel class.
        /// </summary>
        public MongoDbDatabaseViewModel(MongoDbServerViewModel server, string name)
        {
            Server = server;
            Name = name;
            _folders = new ObservableCollection<FolderViewModel>();
            _collections = new FolderViewModel("Collections", this);
            _users = new FolderViewModel("Users", this);
            _folders.Add(_collections);
            _collections.Children.Add(new MongoDbCollectionViewModel(this, null) { IconVisible = false });

            if (_server.ServerVersion < SemanticVersion.Parse("2.6.0"))
            {
                _folders.Add(_users);
                _users.Children.Add(new MongoDbUserViewModel(null, null) { IconVisible = false });
            }

            CreateDatabase = new RelayCommand(InnerCreateDatabase, () =>
            {
                return !string.IsNullOrWhiteSpace(Name) && IsNew;
            });

            RunCommand = new RelayCommand<DatabaseCommand>(InnerOpenRunCommand);

            OpenCreateNewCollection = new RelayCommand<MongoDbDatabaseViewModel>(
            database =>
            {
                Messenger.Default.Send(new NotificationMessage<MongoDbDatabaseViewModel>(this, "OpenCreateNewCollection"));
            });

            Refresh = new RelayCommand(LoadCollections);

            DatabaseCommands = new Dictionary<string, DatabaseCommand>();
            DatabaseCommands.Add("repairDatabase", new DatabaseCommand() { Command = "{ repairDatabase: 1 }" });

            ConfirmDropDatabase = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage<MongoDbDatabaseViewModel>(this, "ConfirmDropDatabase"));
            });

            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, (message) =>
            {
                if (message.Sender == _collections && message.PropertyName == "IsExpanded" && _collections.IsExpanded)
                {
                    if (!IsNew && !_collectionsLoaded && !string.IsNullOrWhiteSpace(Name))
                        LoadCollections();
                }
                else if (Server.ServerVersion < SemanticVersion.Parse("2.6.0") && message.Sender == _users && message.PropertyName == "IsExpanded" && _users.IsExpanded)
                {
                    if (!IsNew && !_usersLoaded && !string.IsNullOrWhiteSpace(Name))
                        LoadUsers();
                }
            });

            Messenger.Default.Register<NotificationMessage<CreateCollectionViewModel>>(this, InnerCreateNewCollection);
            Messenger.Default.Register<NotificationMessage<MongoDbCollectionViewModel>>(this, InnerDropCollection);
        }

        public async void LoadCollections()
        {
            _collections.IsBusy = true;
            try
            {
                var collections = await Server.MongoDbService.GetCollectionsAsync(Name);

                List<MongoDbCollectionViewModel> systemCollections = new List<MongoDbCollectionViewModel>();
                List<MongoDbCollectionViewModel> standardCollections = new List<MongoDbCollectionViewModel>();
                foreach (var collection in collections)
                {
                    var collectionVm = new MongoDbCollectionViewModel(this, collection["name"].AsString);
                    collectionVm.Database = this;

                    if (collection["name"].AsString.StartsWith("system."))
                        systemCollections.Add(collectionVm);
                    else
                        standardCollections.Add(collectionVm);
                }
                FolderViewModel systemCollectionsFolder = new FolderViewModel("System", this);
                foreach (var systemCollection in systemCollections.OrderBy(o => o.Name))
                    systemCollectionsFolder.Children.Add(systemCollection);

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    _collections.Children.Clear();

                    _collections.Children.Add(systemCollectionsFolder);

                    foreach (var collection in standardCollections.OrderBy(o => o.Name))
                        _collections.Children.Add(collection);

                    _collectionsLoaded = true;
                    _collections.ItemsCount = _collections.Children.OfType<MongoDbCollectionViewModel>().Count();
                });
                _collections.IsBusy = false;

                await LoadCollectionsStats(systemCollections.Union(standardCollections));
            }
            catch (Exception ex)
            {
                Utils.LoggerHelper.Logger.Error(string.Format("Failed to get collections on database '{0}', server '{1}'", Name, Server.Name), ex);
            }
            finally
            {
                _collections.IsBusy = false;
            }
        }

        private async Task LoadCollectionsStats(IEnumerable<MongoDbCollectionViewModel> collections)
        {
            foreach (var collection in collections.OrderBy(o => o.Name))
            {
                try
                {
                    await LoadCollectionStats(collection);
                }
                catch (Exception ex)
                {
                    Utils.LoggerHelper.Logger.Error(string.Format("Failed to get collection stats for collection '{0}' on database '{1}', server '{2}'", collection.Name, Name, Server.Name), ex);
                }
            }
        }

        private async Task LoadCollectionStats(MongoDbCollectionViewModel collection)
        {
            collection.Stats = await Server.MongoDbService.ExecuteRawCommandAsync(Name, BsonDocument.Parse("{ collStats: \"" + collection.Name + "\", verbose: true }"), cts.Token);
            switch (collection.Stats["storageSize"].BsonType)
            {
                case MongoDB.Bson.BsonType.Int32:
                    collection.SizeOnDisk = collection.Stats["storageSize"].AsInt32;
                    break;
                case MongoDB.Bson.BsonType.Int64:
                    collection.SizeOnDisk = collection.Stats["storageSize"].AsInt64;
                    break;
                case MongoDB.Bson.BsonType.Double:
                    collection.SizeOnDisk = collection.Stats["storageSize"].AsDouble;
                    break;
            }

            switch (collection.Stats["count"].BsonType)
            {
                case MongoDB.Bson.BsonType.Int32:
                    collection.ItemsCount = collection.Stats["count"].AsInt32;
                    break;
                case MongoDB.Bson.BsonType.Int64:
                    collection.ItemsCount = collection.Stats["count"].AsInt64;
                    break;
            }
        }

        private async void LoadUsers()
        {
            _users.IsBusy = true;
            try
            {
                var usersResult = await Server.MongoDbService.FindAsync(Name, "system.users", null, null, null, null, null, false, Guid.NewGuid(), cts.Token);
                _users.Children.Clear();

                if (usersResult.Count > 0)
                {
                    foreach (var user in usersResult)
                    {
                        _users.Children.Add(new MongoDbUserViewModel(user["name"].AsString, user));
                    }
                }

                _usersLoaded = true;
            }
            catch (Exception ex)
            {
                Utils.LoggerHelper.Logger.Error(string.Format("Failed to get users on server '{0}'", Server.Name), ex);
            }
            finally
            {
                _users.IsBusy = false;
            }
        }

        public RelayCommand CreateDatabase { get; set; }

        public RelayCommand<DatabaseCommand> RunCommand { get; set; }

        public RelayCommand<MongoDbDatabaseViewModel> OpenCreateNewCollection { get; set; }

        public RelayCommand Refresh { get; set; }

        public RelayCommand ConfirmDropDatabase { get; set; }

        public async void InnerCreateDatabase()
        {
            if (IsNew)
            {
                try
                {
                    await Server.MongoDbService.CreateNewDatabaseAsync(this.Name);
                    IsNew = false;
                }
                catch (Exception ex)
                {
                    Utils.LoggerHelper.Logger.Error(string.Format("Failed to create database '{0}' on server '{1}'", Name, Server.Name), ex);
                }
            }
        }

        private void InnerOpenRunCommand(DatabaseCommand param)
        {
            TabViewModel tabVm = SimpleIoc.Default.GetInstanceWithoutCaching<TabViewModel>();
            tabVm.CommandType = CommandType.RunCommand;
            tabVm.Database = this.Name;
            tabVm.Service = this.Server.MongoDbService;
            tabVm.Name = this.Name;
            if (param == null)
                tabVm.Command = "{}";
            else
                tabVm.Command = param.Command;
            Messenger.Default.Send(new NotificationMessage<TabViewModel>(tabVm, "OpenTab"));
        }

        private async void InnerCreateNewCollection(NotificationMessage<CreateCollectionViewModel> message)
        {
            if (message.Notification == "CreateCollection" && message.Target == this)
            {
                try
                {
                    IsBusy = true;
                    MongoDB.Driver.CreateCollectionOptions options = new MongoDB.Driver.CreateCollectionOptions();
                    options.AutoIndexId = message.Content.AutoIndexId;
                    options.Capped = message.Content.Capped;
                    options.MaxDocuments = message.Content.MaxDocuments;
                    options.MaxSize = message.Content.MaxSize;
                    options.UsePowerOf2Sizes = message.Content.UsePowerOf2Sizes;
                    if (!string.IsNullOrWhiteSpace(message.Content.StorageEngine))
                        options.StorageEngine = BsonDocument.Parse(message.Content.StorageEngine);
                    await Server.MongoDbService.CreateCollectionAsync(Name, message.Content.Name, options);

                    var newCollection = new MongoDbCollectionViewModel(this, message.Content.Name);
                    newCollection.IsSelected = true;
                    newCollection.IsBusy = true;
                    this.IsExpanded = true;
                    this._collections.IsExpanded = true;
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        _collections.Children.Add(newCollection);
                        _collections.ItemsCount = _collections.Children.OfType<MongoDbCollectionViewModel>().Count();
                    });
                    await LoadCollectionStats(newCollection);
                }
                catch (Exception ex)
                {
                    Utils.LoggerHelper.Logger.Error(string.Format("Failed to create collection '{0}' on database '{1}', server '{2}'", message.Content.Name, Name, Server.Name), ex);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        public async void InnerDropCollection(NotificationMessage<MongoDbCollectionViewModel> message)
        {
            if (message.Notification == "DropCollection" && message.Target == this)
            {
                IsBusy = true;
                message.Content.IsBusy = true;
                try
                {
                    await Server.MongoDbService.DropCollectionAsync(Name, message.Content.Name);
                    _collections.Children.Remove(message.Content);
                    _collections.ItemsCount = _collections.Children.OfType<MongoDbCollectionViewModel>().Count();
                    message.Content.Cleanup();
                }
                catch (Exception ex)
                {
                    Utils.LoggerHelper.Logger.Error(string.Format("Failed to drop collection '{0}' on database '{1}', server '{2}'", message.Content.Name, Name, Server.Name), ex);
                }
                finally
                {
                    IsBusy = false;
                    message.Content.IsBusy = false;
                }
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            MessengerInstance.Unregister(this);
            foreach (var folder in Folders)
                folder.Cleanup();
            this.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                cts.Dispose();

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }
}