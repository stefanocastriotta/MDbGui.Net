using GalaSoft.MvvmLight;

namespace MongoDbGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MongoDbIndexViewModel : ViewModelBase
    {
        private MongoDbCollectionViewModel _collection;
        public MongoDbCollectionViewModel Collection
        {
            get { return _collection; }
            set
            {
                _collection = value;
                RaisePropertyChanged("Collection");
            }
        }

        /// <summary>
        /// Initializes a new instance of the MongoDbIndexViewModel class.
        /// </summary>
        public MongoDbIndexViewModel()
        {
        }
    }
}