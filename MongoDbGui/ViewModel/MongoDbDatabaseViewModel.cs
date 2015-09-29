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
    public class MongoDbDatabaseViewModel : ViewModelBase
    {
        private MongoDbServerViewModel _server;
        public MongoDbServerViewModel Server
        {
            get { return _server; }
            set
            {
                _server = value;
                RaisePropertyChanged("Server");
            }
        }


        private ObservableCollection<MongoDbCollectionViewModel> _collections;
        public ObservableCollection<MongoDbCollectionViewModel> Collections
        {
            get { return _collections; }
            set
            {
                _collections = value;
                RaisePropertyChanged("Collections");
            }
        }

        /// <summary>
        /// Initializes a new instance of the MongoDbDatabaseViewModel class.
        /// </summary>
        public MongoDbDatabaseViewModel()
        {
            _collections = new ObservableCollection<MongoDbCollectionViewModel>();
        }
    }
}