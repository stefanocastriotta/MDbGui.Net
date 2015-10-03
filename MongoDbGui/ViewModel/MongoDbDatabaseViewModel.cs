using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
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
    public class MongoDbDatabaseViewModel : BaseTreeviewViewModel
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

        private ObservableCollection<FolderViewModel> _folders;
        public ObservableCollection<FolderViewModel> Folders
        {
            get { return _folders; }
            set
            {
                _folders = value;
                RaisePropertyChanged("Folders");
            }
        }

        private FolderViewModel _collections;
        private FolderViewModel _users;

        /// <summary>
        /// Initializes a new instance of the MongoDbDatabaseViewModel class.
        /// </summary>
        public MongoDbDatabaseViewModel(MongoDbServerViewModel server, string name)
        {
            Server = server;
            Name = name;
            _folders = new ObservableCollection<FolderViewModel>();
            _collections = new FolderViewModel("Collections", this);
            _users = new FolderViewModel("Users", this);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                _folders.Add(_collections);
                _collections.Children.Add(new MongoDbCollectionViewModel(this, ""));
                _folders.Add(_users);
                _users.Children.Add(new BaseTreeviewViewModel());
            });

            CreateDatabase = new RelayCommand(InnerCreateDatabase, () =>
            {
                return !string.IsNullOrWhiteSpace(Name) && IsNew;
            });
            CreateNewCollection = new RelayCommand(InnerCreateNewCollection);
            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, (message) =>
            {
                if (message.Sender == _collections && message.PropertyName == "IsExpanded" && _collections.IsExpanded)
                {
                    if (!IsNew && !_collectionsLoaded && !string.IsNullOrWhiteSpace(Name))
                        LoadCollections();
                }
            });
        }

        public async void LoadCollections()
        {
            _collections.IsBusy = true;
            var collections = await Server.MongoDbService.GetCollections(Name);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                _collections.Children.Clear();
                FolderViewModel systemCollections = new FolderViewModel("System", this);
                _collections.Children.Add(systemCollections);
                foreach (var collection in collections)
                {
                    var collectionVm = new MongoDbCollectionViewModel(this, collection["name"].AsString);
                    collectionVm.Database = this;
                    if (collection["name"].AsString.StartsWith("system."))
                        systemCollections.Children.Add(collectionVm);
                    else
                        _collections.Children.Add(collectionVm);
                }
                _collectionsLoaded = true;
                _collections.IsBusy = false;
            });
        }

        public RelayCommand CreateDatabase { get; set; }

        public RelayCommand CreateNewCollection { get; set; }

        public async void InnerCreateDatabase()
        {
            if (IsNew)
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
            this.IsExpanded = true;
            this._collections.IsExpanded = true;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                _collections.Children.Add(newCollection);
            });
        }
    }
}