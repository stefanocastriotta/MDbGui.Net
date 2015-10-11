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
    public class MongoDbServerViewModel : BaseTreeviewViewModel
    {
        public readonly IMongoDbService MongoDbService;

        private ObservableCollection<BaseTreeviewViewModel> _items;
        public ObservableCollection<BaseTreeviewViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                RaisePropertyChanged("Items");
            }
        }

        /// <summary>
        /// Initializes a new instance of the MongoDbServerViewModel class.
        /// </summary>
        public MongoDbServerViewModel(IMongoDbService mongoDbService)
        {
            MongoDbService = mongoDbService;
            _items = new ObservableCollection<BaseTreeviewViewModel>();
            CreateNewDatabase = new RelayCommand(InnerCreateNewDatabase);
            Disconnect = new RelayCommand(InnerDisconnect);
            RunCommand = new RelayCommand(InnerOpenRunCommand);
        }

        public RelayCommand CreateNewDatabase { get; set; }

        public RelayCommand Disconnect { get; set; }

        public RelayCommand RunCommand { get; set; }

        public void InnerCreateNewDatabase()
        {
            var newDb = new MongoDbDatabaseViewModel(this, "");
            newDb.IsSelected = true;
            newDb.IsNew = true;
            this.IsExpanded = true;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Items.Add(newDb);
            });
        }

        public void InnerDisconnect()
        {
            Messenger.Default.Send(new NotificationMessage<MongoDbServerViewModel>(this, "Disconnect"));
        }


        private void InnerOpenRunCommand()
        {
            TabViewModel tabVm = new TabViewModel();
            tabVm.CommandType = "Command";
            tabVm.Server = this;
            tabVm.Database = new MongoDbDatabaseViewModel(this, "admin");
            tabVm.Name = this.Name;
            Messenger.Default.Send(new NotificationMessage<TabViewModel>(tabVm, "OpenTab"));
        }

        public override void Cleanup()
        {
            base.Cleanup();
            MessengerInstance.Unregister(this);
        }
    }
}