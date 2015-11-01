using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using MongoDB.Bson;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace MDbGui.Net.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MongoDbCollectionViewModel : BaseTreeviewViewModel
    {
        protected bool _iconVisible = true;

        public bool IconVisible
        {
            get { return _iconVisible; }
            set
            {
                Set(ref _iconVisible, value);
            }
        }

        private BsonDocument _stats;
        public BsonDocument Stats
        {
            get
            {
                return _stats;
            }
            set
            {
                Set(ref _stats, value);
            }
        }

        private MongoDbDatabaseViewModel _database;
        public MongoDbDatabaseViewModel Database
        {
            get { return _database; }
            set
            {
                Set(ref _database, value);
            }
        }

        private string _oldName = string.Empty;

        private bool _indexesLoaded;

        private FolderViewModel _indexes;

        private ObservableCollection<FolderViewModel> _folders;
        public ObservableCollection<FolderViewModel> Folders
        {
            get { return _folders; }
            set
            {
                Set(ref _folders, value);
            }
        }

        public RelayCommand OpenTab { get; set; }

        public RelayCommand SaveCollection { get; set; }

        public RelayCommand RenameCollection { get; set; }

        public RelayCommand InsertDocuments { get; set; }

        public RelayCommand ConfirmDropCollection { get; set; }

        public RelayCommand CreateIndex { get; set; }

        /// <summary>
        /// Initializes a new instance of the MongoDbCollectionViewModel class.
        /// </summary>
        public MongoDbCollectionViewModel(MongoDbDatabaseViewModel database, string collectionName)
        {
            _folders = new ObservableCollection<FolderViewModel>();
            _indexes = new FolderViewModel("Indexes", this);
            _folders.Add(_indexes);
            _indexes.Children.Add(new MongoDbIndexViewModel(this, null) { IconVisible = false });

            OpenTab = new RelayCommand(InternalOpenTab);
            RenameCollection = new RelayCommand(InternalRenameCollection);
            SaveCollection = new RelayCommand(InnerSaveCollection, () =>
            {
                return !string.IsNullOrWhiteSpace(Name);
            });
            InsertDocuments = new RelayCommand(InternalInsertDocuments);
            CreateIndex = new RelayCommand(InternalCreateIndex);
            ConfirmDropCollection = new RelayCommand(
            () =>
            {
                Messenger.Default.Send(new NotificationMessage<MongoDbCollectionViewModel>(this, ServiceLocator.Current.GetInstance<MainViewModel>(), this, "ConfirmDropCollection"));
            });

            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, (message) =>
            {
                if (message.Sender == _indexes && message.PropertyName == "IsExpanded" && _indexes.IsExpanded)
                {
                    if (!IsNew && !_indexesLoaded && !string.IsNullOrWhiteSpace(Name))
                        LoadIndexes();
                }
            });

            Messenger.Default.Register<NotificationMessage<CreateIndexViewModel>>(this, InnerCreateIndex);

            Messenger.Default.Register<NotificationMessage<MongoDbIndexViewModel>>(this, (message) => IndexMessageHandler(message));

            Database = database;
            _name = collectionName;
            _oldName = collectionName;
        }

        private void InternalOpenTab()
        {
            TabViewModel tabVm = new TabViewModel();
            tabVm.CommandType = Model.CommandType.Find;
            tabVm.Database = this.Database.Name;
            tabVm.Service = this.Database.Server.MongoDbService;
            tabVm.Collection = this.Name;
            tabVm.Name = this.Name;
            tabVm.ExecuteOnOpen = true;
            Messenger.Default.Send(new NotificationMessage<TabViewModel>(tabVm, "OpenTab"));
        }

        private void InternalRenameCollection()
        {
            IsEditing = true;
        }

        public async void InnerSaveCollection()
        {
            try
            {
                IsBusy = true;
                await Database.Server.MongoDbService.RenameCollectionAsync(Database.Name, this._oldName, this.Name);
                _oldName = this.Name;
                IsEditing = false;
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

        private void InternalInsertDocuments()
        {
            TabViewModel tabVm = new TabViewModel();
            tabVm.CommandType = Model.CommandType.Insert;
            tabVm.Database = this.Database.Name;
            tabVm.Service = this.Database.Server.MongoDbService;
            tabVm.Collection = this.Name;
            tabVm.Name = this.Name;
            Messenger.Default.Send(new NotificationMessage<TabViewModel>(tabVm, "OpenTab"));
        }

        private void InternalCreateIndex()
        {
            Messenger.Default.Send(new NotificationMessage<MongoDbCollectionViewModel>(this, "CreateIndex"));
        }

        public async void LoadIndexes()
        {
            _indexes.IsBusy = true;
            try
            {
                var indexes = await Database.Server.MongoDbService.GetCollectionIndexesAsync(Database.Name, Name);
                _indexes.Children.Clear();

                foreach (var index in indexes)
                {
                    var indexVm = new MongoDbIndexViewModel(this, index["name"].AsString);
                    indexVm.IndexDefinition = index["key"].ToJson(new MongoDB.Bson.IO.JsonWriterSettings() { Indent = true });
                    _indexes.Children.Add(indexVm);
                }
                _indexes.ItemsCount = _indexes.Children.OfType<MongoDbIndexViewModel>().Count();
                _indexesLoaded = true;
            }
            catch (Exception ex)
            {
                //TODO: log error
            }
            finally
            {
                _indexes.IsBusy = false;
            }
        }

        private async void InnerCreateIndex(NotificationMessage<CreateIndexViewModel> message)
        {
            if ((message.Notification == "CreateIndex" || message.Notification == "RecreateIndex") && message.Target == this)
            {
                try
                {
                    IsBusy = true;
                    if (message.Notification == "RecreateIndex")
                        await Database.Server.MongoDbService.DropIndexAsync(Database.Name, Name, message.Content.Name);
                    string result = await Database.Server.MongoDbService.CreateIndexAsync(Database.Name, Name, message.Content.IndexDefinition, new MongoDB.Driver.CreateIndexOptions() { Name = message.Content.Name });
                    this.IsExpanded = true;
                    this._indexes.IsExpanded = true;
                    if (message.Notification == "CreateIndex")
                    {
                        var newIndex = new MongoDbIndexViewModel(this, result);
                        newIndex.IndexDefinition = message.Content.IndexDefinition;
                        _indexes.Children.Add(newIndex);
                        _indexes.ItemsCount = _indexes.Children.OfType<MongoDbIndexViewModel>().Count();
                    }
                    else
                    {
                        var existingIndexVm = _indexes.Children.FirstOrDefault(i => i.Name == message.Content.Name);
                        if (existingIndexVm != null)
                            ((MongoDbIndexViewModel)existingIndexVm).IndexDefinition = message.Content.IndexDefinition;
                    }
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

        private async void IndexMessageHandler(NotificationMessage<MongoDbIndexViewModel> message)
        {
            if (message.Notification == "DropIndex" && message.Target == this)
            {
                IsBusy = true;
                message.Content.IsBusy = true;
                try
                {
                    await Database.Server.MongoDbService.DropIndexAsync(Database.Name, Name, message.Content.Name);
                    _indexes.Children.Remove(message.Content);
                    _indexes.ItemsCount = _indexes.Children.OfType<MongoDbCollectionViewModel>().Count();
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
            foreach (var folder in Folders)
                folder.Cleanup();
        }
    }
}