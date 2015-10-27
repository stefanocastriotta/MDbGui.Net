using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using MongoDB.Bson;
using System;
using System.Collections.ObjectModel;

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

        private ObservableCollection<MongoDbIndexViewModel> _indexes;
        public ObservableCollection<MongoDbIndexViewModel> Indexes
        {
            get { return _indexes; }
            set
            {
                _indexes = value;
                RaisePropertyChanged("Indexes");
            }
        }

        public RelayCommand OpenTab { get; set; }

        public RelayCommand SaveCollection { get; set; }

        public RelayCommand RenameCollection { get; set; }

        public RelayCommand InsertDocuments { get; set; }

        public RelayCommand ConfirmDropCollection { get; set; }

        /// <summary>
        /// Initializes a new instance of the MongoDbCollectionViewModel class.
        /// </summary>
        public MongoDbCollectionViewModel(MongoDbDatabaseViewModel database, string collectionName)
        {
            _indexes = new ObservableCollection<MongoDbIndexViewModel>();
            OpenTab = new RelayCommand(InternalOpenTab);
            RenameCollection = new RelayCommand(InternalRenameCollection);
            SaveCollection = new RelayCommand(InnerSaveCollection, () =>
            {
                return !string.IsNullOrWhiteSpace(Name);
            });
            InsertDocuments = new RelayCommand(InternalInsertDocuments);
            ConfirmDropCollection = new RelayCommand(
            () =>
            {
                Messenger.Default.Send(new NotificationMessage<MongoDbCollectionViewModel>(this, ServiceLocator.Current.GetInstance<MainViewModel>(), this, "ConfirmDropCollection"));
            });

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

        public override void Cleanup()
        {
            base.Cleanup();
            MessengerInstance.Unregister(this);
            foreach (var index in Indexes)
                index.Cleanup();
        }
    }
}