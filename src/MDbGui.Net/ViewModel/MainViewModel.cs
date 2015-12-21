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
using MDbGui.Net.Views.Controls;
using MDbGui.Net.Utils;
using MongoDB.Driver.Core.Misc;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using MDbGui.Net.ViewModel.Operations;

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
                Set(ref _activeConnections, value);
            }
        }

        private ObservableCollection<TabViewModel> _tabs;
        public ObservableCollection<TabViewModel> Tabs
        {
            get { return _tabs; }
            set
            {
                Set(ref _tabs, value);
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

        public ObservableCollection<log4net.Core.LoggingEvent> LogEvents { get; set; }

        public RelayCommand<log4net.Core.LoggingEvent> ViewLogDetails { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _activeConnections = new ObservableCollection<MongoDbServerViewModel>();
            _tabs = new ObservableCollection<TabViewModel>();
            LogEvents = LoggerHelper.LogEvents;
            ViewLogDetails = new RelayCommand<log4net.Core.LoggingEvent>((param) =>
            {
                Messenger.Default.Send(new NotificationMessage<log4net.Core.LoggingEvent>(param, Constants.ShowLogDetailsMessage));
            });
            Messenger.Default.Register<NotificationMessage<ConnectionInfo>>(this, (message) => LoggingInMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<TabViewModel>>(this, (message) => TabMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<MongoDbServerViewModel>>(this, (message) => MongoDbServerMessageHandler(message));

            if (IsInDesignModeStatic)
            {
                var _mongoDbService = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<IMongoDbService>();
                MongoDbServerViewModel serverVm = new MongoDbServerViewModel(_mongoDbService);
                serverVm.Name = "127.0.0.1:27017";

                ActiveConnections.Add(serverVm);
                var task = _mongoDbService.ConnectAsync(new ConnectionInfo());
                task.Wait();
                var serverInfo = task.Result;
                serverVm.LoadDatabases(serverInfo.Databases);

                TabViewModel tabDesign1 = SimpleIoc.Default.GetInstanceWithoutCaching<TabViewModel>();
                tabDesign1.Name = "Collection1";
                tabDesign1.SelectedOperation = tabDesign1.FindOperation;
                this.Tabs.Add(tabDesign1);
                this.SelectedTab = tabDesign1;
                TabViewModel tabDesign2 = SimpleIoc.Default.GetInstanceWithoutCaching<TabViewModel>();
                tabDesign2.Name = "localhost:27017";
                tabDesign2.SelectedOperation = tabDesign2.AggregateOperation;
                this.Tabs.Add(tabDesign2);

                LogEvents = new ObservableCollection<log4net.Core.LoggingEvent>();
                LogEvents.Add(new log4net.Core.LoggingEvent(new log4net.Core.LoggingEventData() { Level = log4net.Core.Level.Debug, Message = "Test debug message", TimeStamp = DateTime.Parse("2015/11/09 00:01:00") }));
                LogEvents.Add(new log4net.Core.LoggingEvent(new log4net.Core.LoggingEventData() { Level = log4net.Core.Level.Info, Message = "Test info message", TimeStamp = DateTime.Parse("2015/11/09 00:02:00") }));
                LogEvents.Add(new log4net.Core.LoggingEvent(new log4net.Core.LoggingEventData() { ExceptionString = "TestException", Level = log4net.Core.Level.Warn, Message = "Test warn message", TimeStamp = DateTime.Parse("2015/11/09 00:03:00") }));
                LogEvents.Add(new log4net.Core.LoggingEvent(new log4net.Core.LoggingEventData() { ExceptionString = "TestException", Level = log4net.Core.Level.Error, Message = "Test error message", TimeStamp = DateTime.Parse("2015/11/09 00:04:00") }));
            }

        }

        private async void LoggingInMessageHandler(NotificationMessage<ConnectionInfo> message)
        {
            if (message.Notification == Constants.LoggingInMessage)
            {
                var _mongoDbService = SimpleIoc.Default.GetInstanceWithoutCaching<IMongoDbService>();
                MongoDbServerViewModel serverVm = new MongoDbServerViewModel(_mongoDbService);
                serverVm.IsBusy = true;
                serverVm.Name = message.Content.Address + ":" + message.Content.Port;

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ActiveConnections.Add(serverVm);
                });

                try
                {
                    LoggerHelper.Logger.Info("Connecting to server " + message.Content.Address + ":" + message.Content.Port);
                    var serverInfo = await _mongoDbService.ConnectAsync(message.Content);
                    serverVm.ServerVersion = SemanticVersion.Parse(serverInfo.ServerStatus["version"].AsString);
                    LoggerHelper.Logger.Info("Connected to server " + message.Content.Address + ":" + message.Content.Port);
                    serverVm.LoadDatabases(serverInfo.Databases);
                }
                catch (Exception ex)
                {
                    LoggerHelper.Logger.Error("Failed to connect to server " + message.Content.Address + ":" + message.Content.Port, ex);
                }
                serverVm.IsBusy = false;
            }
        }


        private void TabMessageHandler(NotificationMessage<TabViewModel> message)
        {
            switch (message.Notification)
            {
                case Constants.OpenTab:
                    LoggerHelper.Logger.Debug("OpenTab message received");
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        message.Content.Connections.AddRange(GetActiveConnections());
                        Tabs.Add(message.Content);
                        SelectedTab = message.Content;
                        if (message.Content.ExecuteOnOpen && message.Content.SelectedOperation != null)
                        {
                            switch (message.Content.SelectedOperation.Name)
                            {
                                case Constants.FindOperation:
                                    message.Content.FindOperation.ExecuteFind.Execute(null);
                                    break;
                                case Constants.CommandOperation:
                                    message.Content.CommandOperation.ExecuteCommand.Execute(null);
                                    break;
                            }
                        }
                    });
                    break;
                case Constants.CloseTab:
                    LoggerHelper.Logger.Debug("CloseTab message received");
                    Tabs.Remove(message.Content);
                    message.Content.Cleanup();
                    break;
            }
        }

        private void MongoDbServerMessageHandler(NotificationMessage<MongoDbServerViewModel> message)
        {
            if (message.Notification == Constants.DisconnectMessage)
            {
                LoggerHelper.Logger.Info("Disconnecting from server " + message.Content.Name);
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