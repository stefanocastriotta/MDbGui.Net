using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MongoDB.Bson;
using MongoDB.Driver;
using MDbGui.Net.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MDbGui.Net.ViewModel
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
            Refresh = new RelayCommand(GetDatabases);
            DatabaseCommands = new Dictionary<string, DatabaseCommand>();
            DatabaseCommands.Add("serverStatus", new DatabaseCommand() { Command = "{ serverStatus: 1 }", ExecuteImmediately = true });
            DatabaseCommands.Add("hostInfo", new DatabaseCommand() { Command = "{ hostInfo: 1 }", ExecuteImmediately = true });
            DatabaseCommands.Add("getLog", new DatabaseCommand() { Command = "{ getLog: 'global' }", ExecuteImmediately = true });
            DatabaseCommands.Add("replSetGetStatus", new DatabaseCommand() { Command = "{ replSetGetStatus: 1 }", ExecuteImmediately = true });

            Messenger.Default.Register<NotificationMessage<MongoDbDatabaseViewModel>>(this, InnerDropDatabase);
        }

        public RelayCommand CreateNewDatabase { get; set; }

        public RelayCommand Disconnect { get; set; }

        public RelayCommand<DatabaseCommand> RunCommand { get; set; }

        public RelayCommand Refresh { get; set; }

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
            tabVm.Service = this.MongoDbService;
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

        private async void GetDatabases()
        {
            try
            {
                this.IsBusy = true;
                var databases = await MongoDbService.ListDatabasesAsync();
                LoadDatabases(databases);
            }
            catch (Exception ex)
            {
                //TODO: log error
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public void LoadDatabases(List<BsonDocument> databases)
        {
            try
            {
                this.Items.Clear();
                List<MongoDbDatabaseViewModel> systemDatabases = new List<MongoDbDatabaseViewModel>();
                List<MongoDbDatabaseViewModel> standardDatabases = new List<MongoDbDatabaseViewModel>();

                FolderViewModel systemDbFolder = new FolderViewModel("System", this);
                foreach (var database in databases)
                {
                    var databaseVm = new MongoDbDatabaseViewModel(this, database["name"].AsString);
                    databaseVm.SizeOnDisk = database["sizeOnDisk"].AsDouble;
                    if (databaseVm.Name == "local")
                        systemDatabases.Add(databaseVm);
                    else
                        standardDatabases.Add(databaseVm);
                }

                foreach (var systemDb in systemDatabases.OrderBy(o => o.Name))
                    systemDbFolder.Children.Add(systemDb);

                this.Items.Add(systemDbFolder);

                foreach (var db in standardDatabases.OrderBy(o => o.Name))
                    this.Items.Add(db);

                this.IsExpanded = true;
            }
            catch (Exception ex)
            {
                //TODO: log error
            }
        }

        public async void InnerDropDatabase(NotificationMessage<MongoDbDatabaseViewModel> message)
        {
            if (message.Notification == "DropDatabase" && message.Target == this)
            {
                IsBusy = true;
                message.Content.IsBusy = true;
                try
                {
                    await MongoDbService.DropDatabaseAsync(message.Content.Name);
                    Items.Remove(message.Content);
                    message.Content.Cleanup();
                }
                catch (Exception ex)
                {
                    //TODO: log error
                }
                finally
                {
                    IsBusy = false;
                    message.Content.IsBusy = false;
                }
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            foreach (var item in Items)
                item.Cleanup();
            MessengerInstance.Unregister(this);
        }
    }
}