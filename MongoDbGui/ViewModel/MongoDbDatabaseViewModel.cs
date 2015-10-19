using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MongoDbGui.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using System;

namespace MongoDbGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MongoDbDatabaseViewModel : BaseTreeviewViewModel
    {
        public Dictionary<string, DatabaseCommand> DatabaseCommands { get; set; }

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
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                _folders.Add(_collections);
                _collections.Children.Add(new MongoDbCollectionViewModel(this, null) { IconVisible = false });
                _folders.Add(_users);
                _users.Children.Add(new BaseTreeviewViewModel());
            });

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

            DatabaseCommands = new Dictionary<string, DatabaseCommand>();
            DatabaseCommands.Add("repairDatabase", new DatabaseCommand() { Command = "{ repairDatabase: 1 }" });

            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, (message) =>
            {
                if (message.Sender == _collections && message.PropertyName == "IsExpanded" && _collections.IsExpanded)
                {
                    if (!IsNew && !_collectionsLoaded && !string.IsNullOrWhiteSpace(Name))
                        LoadCollections();
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
                    _collections.Count = _collections.Children.OfType<MongoDbCollectionViewModel>().Count();
                });

                await LoadCollectionsStats(systemCollections.Union(standardCollections));
            }
            catch (Exception ex)
            {
                //TODO: log error
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
                    //TODO: log error
                }
            }
        }

        private async Task LoadCollectionStats(MongoDbCollectionViewModel collection)
        {
            collection.Stats = await Server.MongoDbService.ExecuteRawCommandAsync(Name, "{ collStats: \"" + collection.Name + "\", verbose: true }");
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
        }

        public RelayCommand CreateDatabase { get; set; }

        public RelayCommand<DatabaseCommand> RunCommand { get; set; }

        public RelayCommand<MongoDbDatabaseViewModel> OpenCreateNewCollection { get; set; }

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
                    //TODO: log error
                }
            }
        }

        private void InnerOpenRunCommand(DatabaseCommand param)
        {
            TabViewModel tabVm = new TabViewModel();
            tabVm.CommandType = CommandType.RunCommand;
            tabVm.Database = this.Name;
            tabVm.Server = this.Server;
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
                        _collections.Count = _collections.Children.OfType<MongoDbCollectionViewModel>().Count();
                    });
                    await LoadCollectionStats(newCollection);
                }
                catch (Exception ex)
                {
                    //TODO: log error
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
                    _collections.Count = _collections.Children.OfType<MongoDbCollectionViewModel>().Count();
                    message.Content.Cleanup();
                }
                catch (Exception ex)
                {
                    //TODO: log error
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
        }
    }
}