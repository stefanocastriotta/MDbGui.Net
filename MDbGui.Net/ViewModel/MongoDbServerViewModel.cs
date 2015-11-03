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
using System.Threading;
using MongoDB.Driver.Core.Misc;

namespace MDbGui.Net.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MongoDbServerViewModel : BaseTreeviewViewModel, IDisposable
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        private bool _disposed;

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

        private SemanticVersion _serverVersion;
        public SemanticVersion ServerVersion
        {
            get
            {
                return _serverVersion;
            }
            set
            {
                Set(ref _serverVersion, value);
            }
        }

        private bool _usersLoaded;

        private FolderViewModel _users;

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
            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, (message) =>
            {
                if (message.Sender == _users && message.PropertyName == "IsExpanded" && _users.IsExpanded)
                {
                    if (!IsNew && !_usersLoaded && !string.IsNullOrWhiteSpace(Name))
                        LoadUsers();
                }
            });
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
                Utils.LoggerHelper.Logger.Debug("Adding new database to server " + Name);
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
            Utils.LoggerHelper.Logger.Debug("Opening RunCommandTab for server " + Name);
            Messenger.Default.Send(new NotificationMessage<TabViewModel>(tabVm, "OpenTab"));
        }

        private async void GetDatabases()
        {
            try
            {
                this.IsBusy = true;
                Utils.LoggerHelper.Logger.Debug("Getting database list of server " + Name);
                var databases = await MongoDbService.ListDatabasesAsync();
                Utils.LoggerHelper.Logger.Debug("Database list of server " + Name + " received");
                LoadDatabases(databases);
            }
            catch (Exception ex)
            {
                Utils.LoggerHelper.Logger.Error("Failed to get databases on server " + Name, ex);
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
                Utils.LoggerHelper.Logger.Debug("Loading database list of server " + Name);
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

                if (ServerVersion >= SemanticVersion.Parse("2.6.0"))
                {
                    _users = new FolderViewModel("Users", this);
                    _users.Children.Add(new MongoDbUserViewModel(null, null) { IconVisible = false });
                    this.Items.Add(_users);
                }

                this.IsExpanded = true;
            }
            catch (Exception ex)
            {
                Utils.LoggerHelper.Logger.Error("Failed to load database list on server " + Name, ex);
            }
        }

        public async void InnerDropDatabase(NotificationMessage<MongoDbDatabaseViewModel> message)
        {
            if (message.Notification == "DropDatabase" && message.Target == this)
            {
                Utils.LoggerHelper.Logger.DebugFormat("DropDatabase notification received on server '{0}', database name '{1}'", Name, message.Content.Name);
                IsBusy = true;
                message.Content.IsBusy = true;
                try
                {
                    Utils.LoggerHelper.Logger.Info("Dropping database " + message.Content.Name + " on server " + Name);
                    await MongoDbService.DropDatabaseAsync(message.Content.Name);
                    Items.Remove(message.Content);
                    message.Content.Cleanup();
                }
                catch (Exception ex)
                {
                    Utils.LoggerHelper.Logger.Error(string.Format("Error while dropping database '{0}' on server '{1}'", message.Content.Name, Name), ex);
                }
                finally
                {
                    IsBusy = false;
                    message.Content.IsBusy = false;
                }
            }
        }

        private async void LoadUsers()
        {
            _users.IsBusy = true;
            try
            {
                var usersResult = await MongoDbService.ExecuteRawCommandAsync("admin", "{ usersInfo: 1 }", cts.Token);
                _users.Children.Clear();

                if (usersResult.Contains("users") && usersResult["users"].AsBsonArray.Count > 0)
                {
                    foreach (var user in usersResult["users"].AsBsonArray)
                    {
                        _users.Children.Add(new MongoDbUserViewModel(user["name"].AsString, user.AsBsonDocument));
                    }
                }

                _usersLoaded = true;
            }
            catch (Exception ex)
            {
                Utils.LoggerHelper.Logger.Error(string.Format("Failed to get users on server '{0}'", Name), ex);
            }
            finally
            {
                _users.IsBusy = false;
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            foreach (var item in Items)
                item.Cleanup();
            MessengerInstance.Unregister(this);
            this.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                cts.Dispose();

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }
}