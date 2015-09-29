using GalaSoft.MvvmLight;
using MongoDB.Driver;
using System.Collections.ObjectModel;

namespace MongoDbGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MongoDbServerViewModel : ViewModelBase
    {
        public MongoClient Client { get; private set; }

        private ObservableCollection<MongoDbDatabaseViewModel> _databases;
        public ObservableCollection<MongoDbDatabaseViewModel> Databases
        {
            get { return _databases; }
            set
            {
                _databases = value;
                RaisePropertyChanged("Databases");
            }
        }

        /// <summary>
        /// Initializes a new instance of the MongoDbServerViewModel class.
        /// </summary>
        public MongoDbServerViewModel(MongoClient client)
        {
            Client = client;
            _databases = new ObservableCollection<MongoDbDatabaseViewModel>();
        }
    }
}