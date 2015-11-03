using System.Windows;
using MDbGui.Net.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using MDbGui.Net.Views.Dialogs;
using MDbGui.Net.Views.Controls;
using MongoDB.Bson;
using MDbGui.Net.Utils;
using System;
using log4net.Core;

namespace MDbGui.Net.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            Messenger.Default.Register<NotificationMessage<MongoDbDatabaseViewModel>>(this, (message) => DatabaseMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<MongoDbCollectionViewModel>>(this, (message) => CollectionMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<MongoDbIndexViewModel>>(this, (message) => IndexMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<DocumentResultViewModel>>(this, (message) => DocumentMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<log4net.Core.LoggingEvent>>(this, (message) => LogDetailsMessageHandler(message));
            Utils.LoggerHelper.Logger.Debug("Application started");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoginView wnd = new LoginView();
            wnd.ShowDialog();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginView wnd = new LoginView();
            wnd.ShowDialog();
        }

        private void CollectionMessageHandler(NotificationMessage<MongoDbCollectionViewModel> message)
        {
            Utils.LoggerHelper.Logger.Debug("MongoDbCollectionViewModel message received");
            switch (message.Notification)
            {
                case "ConfirmDropCollection":
                    var result = MessageBox.Show("Drop collection " + message.Content.Name + "?", "Drop confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Utils.LoggerHelper.Logger.Debug("Sending DropCollection message, collection name:" + message.Content.Name);
                        Messenger.Default.Send(new NotificationMessage<MongoDbCollectionViewModel>(this, message.Content.Database, message.Content, "DropCollection"));
                    }
                    break;
                case "CreateIndex":
                    CreateIndexDialog wnd = new CreateIndexDialog();
                    wnd.Title = "Create index";
                    var vm = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<CreateIndexViewModel>();
                    vm.Collection = message.Content;
                    vm.IsNew = true;
                    wnd.DataContext = vm;
                    wnd.ShowDialog();
                    break;
            }
        }

        private void IndexMessageHandler(NotificationMessage<MongoDbIndexViewModel> message)
        {
            switch (message.Notification)
            {
                case "ConfirmDropIndex":
                    var result = MessageBox.Show("Drop index " + message.Content.Name + "?", "Drop confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Utils.LoggerHelper.Logger.Debug("Sending DropIndex message, index name:" + message.Content.Name);
                        Messenger.Default.Send(new NotificationMessage<MongoDbIndexViewModel>(this, message.Content.Collection, message.Content, "DropIndex"));
                    }
                    break;
                case "EditIndex":
                    CreateIndexDialog wnd = new CreateIndexDialog();
                    wnd.Title = "Edit index";
                    var vm = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<CreateIndexViewModel>();
                    vm.Collection = message.Content.Collection;
                    vm.Name = message.Content.Name;
                    vm.IsNew = false;
                    vm.IndexDefinition = message.Content.IndexDefinition;
                    wnd.DataContext = vm;
                    wnd.ShowDialog();
                    break;
            }
        }

        private void DatabaseMessageHandler(NotificationMessage<MongoDbDatabaseViewModel> message)
        {
            switch (message.Notification)
            {
                case "OpenCreateNewCollection":
                    CreateCollectionDialog wnd = new CreateCollectionDialog();
                    var vm = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<CreateCollectionViewModel>();
                    vm.Database = message.Content;
                    wnd.DataContext = vm;
                    wnd.ShowDialog();
                    break;
                case "ConfirmDropDatabase":
                    var result = MessageBox.Show("Drop database " + message.Content.Name + "?", "Drop confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Messenger.Default.Send(new NotificationMessage<MongoDbDatabaseViewModel>(this, message.Content.Server, message.Content, "DropDatabase"));
                    }
                    break;
            }
        }

        private void DocumentMessageHandler(NotificationMessage<DocumentResultViewModel> message)
        {
            if (message.Notification == "EditResult")
            {
                UpdateDocumentView wnd = new UpdateDocumentView();
                var vm = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<ReplaceOneViewModel>();
                vm.Document = message.Content;
                vm.Replacement = message.Content.Result.ToJson(new JsonWriterSettingsExtended() { Indent = true, UseLocalTime = true });
                wnd.DataContext = vm;
                wnd.ShowDialog();
            }
        }

        private void LogDetailsMessageHandler(NotificationMessage<LoggingEvent> message)
        {
            LogDetailsView logDetails = new LogDetailsView();
            logDetails.DataContext = message.Content;
            logDetails.ShowDialog();
        }
    }
}