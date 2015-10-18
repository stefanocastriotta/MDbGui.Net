using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace MongoDbGui.ViewModel
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

        protected string _options = string.Empty;

        public string Options
        {
            get
            {
                return _options;
            }
            set
            {
                Set(ref _options, value);
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
            Messenger.Default.Send(new NotificationMessage<CreateCollectionViewModel>(this, this, "CreateCollection"));
        }
    }
}