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

        public Dictionary<string, DatabaseCommand> DatabaseCommands { get; set; }


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
            RunCommand = new RelayCommand<DatabaseCommand>(InnerOpenRunCommand);
            DatabaseCommands = new Dictionary<string, DatabaseCommand>();
            DatabaseCommands.Add("serverStatus", new DatabaseCommand() { Command = "{ serverStatus: 1 }", ExecuteImmediately = true });
            DatabaseCommands.Add("hostInfo", new DatabaseCommand() { Command = "{ hostInfo: 1 }", ExecuteImmediately = true });
            DatabaseCommands.Add("replSetGetStatus", new DatabaseCommand() { Command = "{ replSetGetStatus: 1 }", ExecuteImmediately = true });
        }

        public RelayCommand CreateNewDatabase { get; set; }

        public RelayCommand Disconnect { get; set; }

        public RelayCommand<DatabaseCommand> RunCommand { get; set; }

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


        private void InnerOpenRunCommand(DatabaseCommand param)
        {
            TabViewModel tabVm = new TabViewModel();
            tabVm.CommandType = CommandType.RunCommand;
            tabVm.Server = this;
            tabVm.Database = "admin";
            tabVm.Name = this.Name;
            if (param == null)
                tabVm.Command = "{}";
            else
            {
                tabVm.Command = param.Command;
                tabVm.ExecuteOnOpen = param.ExecuteImmediately;
            }
            tabVm.SelectedViewIndex = 1;
            Messenger.Default.Send(new NotificationMessage<TabViewModel>(tabVm, "OpenTab"));
        }

        public override void Cleanup()
        {
            base.Cleanup();
            MessengerInstance.Unregister(this);
        }
    }
}