using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using MongoDbGui.Model;
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
        private readonly IMongoDbService _mongoDbService;

        private MongoDbServerViewModel _server;
        public MongoDbServerViewModel Server
        {
            get { return _server; }
            set
            {
                Set(ref _server, value);
            }
        }

        private bool _collectionsLoaded;

        private bool _isSelected;

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                Set(ref _isSelected, value);
            }
        }

        private bool _isExpanded;

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                Set(ref _isExpanded, value);
                if (!_collectionsLoaded)
                {
                    LoadCollections();
                }
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
        public MongoDbDatabaseViewModel(IMongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
            _collections = new ObservableCollection<MongoDbCollectionViewModel>();
            _collections.Add(new MongoDbCollectionViewModel());
        }

        public async void LoadCollections()
        {
            var collections = await _mongoDbService.GetCollections(Server.Client, Name);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Collections.Clear();
                foreach (var collection in collections)
                {
                    var collectionVm = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<MongoDbCollectionViewModel>();
                    collectionVm.Database = this;
                    collectionVm.Name = collection["name"].AsString;
                    Collections.Add(collectionVm);
                }
                _collectionsLoaded = true;
            });
        }
    }
}