using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MongoDB.Driver;
using MongoDbGui.Model;
using System.Collections.Generic;
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
        public readonly IMongoDbService MongoDbService;

        private string _address = string.Empty;

        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                Set(ref _address, value);
            }
        }

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
            }
        }

        /// <summary>
        /// Initializes a new instance of the MongoDbServerViewModel class.
        /// </summary>
        public MongoDbServerViewModel(IMongoDbService mongoDbService)
        {
            MongoDbService = mongoDbService;
            _databases = new ObservableCollection<MongoDbDatabaseViewModel>();
            CreateNewDatabase = new RelayCommand(InnerCreateNewDatabase);
        }

        public RelayCommand CreateNewDatabase { get; set; }

        public void InnerCreateNewDatabase()
        {
            var newDb = new MongoDbDatabaseViewModel(this, "");
            newDb.IsSelected = true;
            newDb.IsNew = true;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Databases.Add(newDb);
            });
        }

    }
}