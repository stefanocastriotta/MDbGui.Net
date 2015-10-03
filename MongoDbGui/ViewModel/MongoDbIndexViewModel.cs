using GalaSoft.MvvmLight;

namespace MongoDbGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MongoDbIndexViewModel : BaseTreeviewViewModel
    {
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

        /// <summary>
        /// Initializes a new instance of the MongoDbIndexViewModel class.
        /// </summary>
        public MongoDbIndexViewModel()
        {
        }
    }
}