using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace MongoDbGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MongoDbCollectionViewModel : ViewModelBase
    {
        private MongoDbDatabaseViewModel _database;
        public MongoDbDatabaseViewModel Database
        {
            get { return _database; }
            set
            {
                _database = value;
                RaisePropertyChanged("Database");
            }
        }

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

        /// <summary>
        /// Initializes a new instance of the MongoDbCollectionViewModel class.
        /// </summary>
        public MongoDbCollectionViewModel()
        {
            _indexes = new ObservableCollection<MongoDbIndexViewModel>();
        }
    }
}