using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MDbGui.Net.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MDbGui.Net.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<MongoDbServerViewModel> _activeConnections;
        public ObservableCollection<MongoDbServerViewModel> ActiveConnections
        {
            get { return _activeConnections; }
            set
            {
                _activeConnections = value;
                RaisePropertyChanged("ActiveConnections");
            }
        }

        private ObservableCollection<TabViewModel> _tabs;
        public ObservableCollection<TabViewModel> Tabs
        {
            get { return _tabs; }
            set
            {
                _tabs = value;
                RaisePropertyChanged("Tabs");
            }
        }

        private TabViewModel _selectedTab;

        public TabViewModel SelectedTab
        {
            get
            {
                return _selectedTab;
            }
            set
            {
                Set(ref _selectedTab, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _activeConnections = new ObservableCollection<MongoDbServerViewModel>();
            _tabs = new ObservableCollection<TabViewModel>();
            Messenger.Default.Register<NotificationMessage<ConnectionInfo>>(this, (message) => LoggingInMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<TabViewModel>>(this, (message) => TabMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<MongoDbServerViewModel>>(this, (message) => MongoDbServerMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<DocumentResultViewModel>>(this, (message) => DocumentMessageHandler(message));
        }

        private async void LoggingInMessageHandler(NotificationMessage<ConnectionInfo> message)
        {
            if (message.Notification == "LoggingIn")
            {
                var _mongoDbService = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<IMongoDbService>();
                MongoDbServerViewModel serverVm = new MongoDbServerViewModel(_mongoDbService);
                serverVm.IsBusy = true;
                serverVm.Name = message.Content.Address + ":" + message.Content.Port;

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ActiveConnections.Add(serverVm);
                });

                try
                {
                    var serverInfo = await _mongoDbService.ConnectAsync(message.Content);
                    serverVm.LoadDatabases(serverInfo.Databases);
                }
                catch (Exception ex)
                {
                    //TODO: log error
                }
                serverVm.IsBusy = false;
            }
        }


        private void TabMessageHandler(NotificationMessage<TabViewModel> message)
        {
            switch (message.Notification)
            {
                case "OpenTab":
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    message.Content.Connections.AddRange(GetActiveConnections());
                    Tabs.Add(message.Content);
                    SelectedTab = message.Content;
                    if (message.Content.ExecuteOnOpen)
                    {
                        switch (message.Content.CommandType)
                        {
                            case CommandType.Find:
                                message.Content.ExecuteFind.Execute(null);
                                break;
                            case CommandType.RunCommand:
                                message.Content.ExecuteCommand.Execute(null);
                                break;
                        }
                    }
                });
                break;
                case "CloseTab":
                Tabs.Remove(message.Content);
                message.Content.Cleanup();
                break;
            }
        }

        private void DocumentMessageHandler(NotificationMessage<DocumentResultViewModel> message)
        {
            if (message.Notification == "EditResult")
            {
                TabViewModel tabVm = new TabViewModel();
                tabVm.CommandType = MDbGui.Net.Model.CommandType.Replace;
                tabVm.Database = message.Content.Database;
                tabVm.Connections.AddRange(GetActiveConnections());
                tabVm.Service = message.Content.Service;
                tabVm.Collection = message.Content.Collection;
                tabVm.Name = message.Content.Collection;
                tabVm.ReplaceFilter = "{ _id: ObjectId(\"" + message.Content.Id +  "\") }";
                tabVm.Replacement = message.Content.Result.ToJson(new MongoDB.Bson.IO.JsonWriterSettings() { Indent = true });
                Tabs.Insert(Tabs.IndexOf(SelectedTab) + 1, tabVm);
                SelectedTab = tabVm;
            }
        }

        private void MongoDbServerMessageHandler(NotificationMessage<MongoDbServerViewModel> message)
        {
            if (message.Notification == "Disconnect")
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ActiveConnections.Remove(message.Content);
                    message.Content.Cleanup();
                });
            }
        }

        private IEnumerable<ActiveConnection> GetActiveConnections()
        {
            foreach (var connection in ActiveConnections)
            {
                yield return new ActiveConnection() { Name = connection.Name, Service = connection.MongoDbService };
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}