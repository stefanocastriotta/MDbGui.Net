using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
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
                Set(ref _database, value);
            }
        }

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
                //if (!_collectionsLoaded)
                //{
                //    LoadCollections();
                //}
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

        public RelayCommand OpenTab { get; set; }

        /// <summary>
        /// Initializes a new instance of the MongoDbCollectionViewModel class.
        /// </summary>
        public MongoDbCollectionViewModel()
        {
            _indexes = new ObservableCollection<MongoDbIndexViewModel>();
            OpenTab = new RelayCommand(InternalOpenTab);
        }

        private void InternalOpenTab()
        {
            CollectionTabViewModel collectionTabVm = SimpleIoc.Default.GetInstanceWithoutCaching<CollectionTabViewModel>();
            collectionTabVm.Collection = this;
            collectionTabVm.Name = this.Name;
            Messenger.Default.Send(new NotificationMessage<CollectionTabViewModel>(collectionTabVm, "OpenCollectionTab"));
        }
    }
}