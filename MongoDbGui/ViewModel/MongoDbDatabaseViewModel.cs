﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MongoDbGui.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

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
            CreateNewCollection = new RelayCommand(InnerCreateNewCollection);

            RunCommand = new RelayCommand(InnerOpenRunCommand);

            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, (message) =>
            {
                if (message.Sender == _collections && message.PropertyName == "IsExpanded" && _collections.IsExpanded)
                {
                    if (!IsNew && !_collectionsLoaded && !string.IsNullOrWhiteSpace(Name))
                        LoadCollections();
                }
            });
        }

        public async void LoadCollections()
        {
            _collections.IsBusy = true;
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
                _collections.IsBusy = false;
            });

            await LoadCollectionsStats(systemCollections.Union(standardCollections));
        }

        private async Task LoadCollectionsStats(IEnumerable<MongoDbCollectionViewModel> collections)
        {
            foreach (var collection in collections.OrderBy(o => o.Name))
            {
                var stats = await Server.MongoDbService.ExecuteRawCommandAsync(Name, "{ collStats: \"" + collection.Name + "\", verbose: true }");
                switch (stats["storageSize"].BsonType)
                {
                    case MongoDB.Bson.BsonType.Int32:
                        collection.SizeOnDisk = stats["storageSize"].AsInt32;
                        break;
                    case MongoDB.Bson.BsonType.Int64:
                        collection.SizeOnDisk = stats["storageSize"].AsInt64;
                        break;
                    case MongoDB.Bson.BsonType.Double:
                        collection.SizeOnDisk = stats["storageSize"].AsDouble;
                        break;
                }
            }
        }

        public RelayCommand CreateDatabase { get; set; }

        public RelayCommand CreateNewCollection { get; set; }

        public RelayCommand RunCommand { get; set; }

        public async void InnerCreateDatabase()
        {
            if (IsNew)
            {
                await Server.MongoDbService.CreateNewDatabaseAsync(this.Name);
                IsNew = false;
            }
        }

        public void InnerCreateNewCollection()
        {
            var newCollection = new MongoDbCollectionViewModel(this, "");
            newCollection.IsSelected = true;
            newCollection.IsNew = true;
            newCollection.IsEditing = true;
            this.IsExpanded = true;
            this._collections.IsExpanded = true;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                _collections.Children.Add(newCollection);
            });
        }

        private void InnerOpenRunCommand()
        {
            TabViewModel tabVm = new TabViewModel();
            tabVm.CommandType = "Command";
            tabVm.Database = this;
            tabVm.Server = this.Server;
            tabVm.Name = this.Name;
            Messenger.Default.Send(new NotificationMessage<TabViewModel>(tabVm, "OpenTab"));
        }

        public override void Cleanup()
        {
            base.Cleanup();
            MessengerInstance.Unregister(this);
        }
    }
}