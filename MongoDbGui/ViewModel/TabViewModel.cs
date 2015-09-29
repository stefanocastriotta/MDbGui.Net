using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MongoDbGui.Model;

namespace MongoDbGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class TabViewModel : ViewModelBase
    {
        private readonly IMongoDbService _mongoDbService;

        private MongoDbCollectionViewModel _collection;
        public MongoDbCollectionViewModel Collection
        {
            get { return _collection; }
            set
            {
                Set(ref _collection, value);
            }
        }

        private string _name = string.Empty;

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

        private string _command = string.Empty;

        public string Command
        {
            get
            {
                return _command;
            }
            set
            {
                Set(ref _command, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the TabViewModel class.
        /// </summary>
        public TabViewModel(IMongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
            ExecuteCommand = new RelayCommand(InnerExecuteCommand, () =>
            {
                return !string.IsNullOrWhiteSpace(Command);
            });
        }

        public RelayCommand ExecuteCommand { get; set; }

        public async void InnerExecuteCommand()
        {
            
        }
    }
}