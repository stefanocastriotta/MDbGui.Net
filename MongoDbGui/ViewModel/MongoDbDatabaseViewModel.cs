using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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

        private bool _isNew;

        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                Set(ref _isNew, value);
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
        public MongoDbDatabaseViewModel(MongoDbServerViewModel server, string name)
        {
            Server = server;
            Name = name;
            _collections = new ObservableCollection<MongoDbCollectionViewModel>();
            _collections.Add(new MongoDbCollectionViewModel(this, ""));
            CreateDatabase = new RelayCommand(InnerCreateDatabase, () =>
            {
                return !string.IsNullOrWhiteSpace(Name) && IsNew;
            });
            CreateNewCollection = new RelayCommand(InnerCreateNewCollection);
        }

        public async void LoadCollections()
        {
            var collections = await Server.MongoDbService.GetCollections(Name);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Collections.Clear();
                foreach (var collection in collections)
                {
                    var collectionVm = new MongoDbCollectionViewModel(this, collection["name"].AsString);
                    collectionVm.Database = this;
                    Collections.Add(collectionVm);
                }
                _collectionsLoaded = true;
            });
        }

        public RelayCommand CreateDatabase { get; set; }

        public RelayCommand CreateNewCollection { get; set; }

        public async void InnerCreateDatabase()
        {
            if (_isNew)
            {
                await Server.MongoDbService.CreateNewDatabase(this.Name);
                IsNew = false;
            }
        }

        public void InnerCreateNewCollection()
        {
            var newCollection = new MongoDbCollectionViewModel(this, "");
            newCollection.IsSelected = true;
            newCollection.IsNew = true;
            newCollection.IsEditing = true;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Collections.Add(newCollection);
            });
        }
    }
}