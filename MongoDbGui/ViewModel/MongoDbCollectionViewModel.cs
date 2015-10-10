using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System.Collections.ObjectModel;

namespace MongoDbGui.ViewModel
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

            Database = database;
            _name = collectionName;
            _oldName = collectionName;
        }

        private void InternalOpenTab()
        {
            CollectionTabViewModel collectionTabVm = new CollectionTabViewModel();
            collectionTabVm.Collection = this;
            collectionTabVm.Name = this.Name;
            Messenger.Default.Send(new NotificationMessage<CollectionTabViewModel>(collectionTabVm, "OpenCollectionTab"));
        }

        private void InternalRenameCollection()
        {
            IsEditing = true;
        }

        public async void InnerSaveCollection()
        {
            if (IsNew)
            {
                await Database.Server.MongoDbService.CreateCollectionAsync(Database.Name, this.Name);
                IsNew = false;
            }
            else
            {
                await Database.Server.MongoDbService.RenameCollectionAsync(Database.Name, this._oldName, this.Name);
            }
            _oldName = this.Name;
            IsEditing = false;
        }

        private void InternalInsertDocuments()
        {
            Messenger.Default.Send(new NotificationMessage<MongoDbCollectionViewModel>(this, "OpenInsertDocuments"));
        }


        public override void Cleanup()
        {
            base.Cleanup();
            MessengerInstance.Unregister(this);
        }
    }
}