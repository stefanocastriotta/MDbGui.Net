using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace MDbGui.Net.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CreateCollectionViewModel : ViewModelBase
    {
        private MongoDbDatabaseViewModel _database;
        public MongoDbDatabaseViewModel Database
        {
            get { return _database; }
            set
            {
                Set(ref _database, value);
            }
        }

        protected string _name = string.Empty;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                Set(ref _name, value);
            }
        }

        private bool? _autoIndexId;
        public bool? AutoIndexId
        {
            get
            {
                return _autoIndexId;
            }
            set
            {
                Set(ref _autoIndexId, value);
            }
        }

        private bool? _capped;
        public bool? Capped
        {
            get
            {
                return _capped;
            }
            set
            {
                Set(ref _capped, value);
            }
        }

        private long? _maxDocuments;
        public long? MaxDocuments
        {
            get
            {
                return _maxDocuments;
            }
            set
            {
                Set(ref _maxDocuments, value);
            }
        }

        private long? _maxSize;
        public long? MaxSize
        {
            get
            {
                return _maxSize;
            }
            set
            {
                Set(ref _maxSize, value);
            }
        }

        private bool? _usePowerOf2Sizes;
        public bool? UsePowerOf2Sizes
        {
            get
            {
                return _usePowerOf2Sizes;
            }
            set
            {
                Set(ref _usePowerOf2Sizes, value);
            }
        }

        protected string _storageEngine = string.Empty;
        public string StorageEngine
        {
            get
            {
                return _storageEngine;
            }
            set
            {
                Set(ref _storageEngine, value);
            }
        }

        public RelayCommand CreateCollection { get; set; }

        /// <summary>
        /// Initializes a new instance of the CreateCollectionViewModel class.
        /// </summary>
        public CreateCollectionViewModel()
        {
            CreateCollection = new RelayCommand(InnerCreateCollection, () =>
            {
                return !string.IsNullOrWhiteSpace(Name);
            });
        }

        public void InnerCreateCollection()
        {
            Messenger.Default.Send(new NotificationMessage<CreateCollectionViewModel>(this, Database, this, "CreateCollection"));
        }

        public override void Cleanup()
        {
            base.Cleanup();
            MessengerInstance.Unregister(this);
        }
    }
}