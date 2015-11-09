using GalaSoft.MvvmLight.Messaging;
using MDbGui.Net.Utils;
using MDbGui.Net.ViewModel;
using MDbGui.Net.Views.Dialogs;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MDbGui.Net.Views.Controls
{
    /// <summary>
    /// Interaction logic for ServerTreeView.xaml
    /// </summary>
    public partial class ServerTreeView : UserControl
    {
        public ServerTreeView()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage<MongoDbDatabaseViewModel>>(this, (message) => DatabaseMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<MongoDbCollectionViewModel>>(this, (message) => CollectionMessageHandler(message));
            Messenger.Default.Register<NotificationMessage<MongoDbIndexViewModel>>(this, (message) => IndexMessageHandler(message));
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Grid && ((TreeView)e.Source).Items.Count > 0)
            {
                foreach (var item in ((TreeView)e.Source).Items.Cast<BaseTreeviewViewModel>())
                {
                    item.IsSelected = false;
                    item.UnselectAll();
                }
                ((Grid)e.OriginalSource).Focus();
            }
        }

        private void CollectionMessageHandler(NotificationMessage<MongoDbCollectionViewModel> message)
        {
            LoggerHelper.Logger.Debug("MongoDbCollectionViewModel message received");
            switch (message.Notification)
            {
                case Constants.ConfirmDropCollectionMessage:
                    var result = MessageBox.Show("Drop collection " + message.Content.Name + "?", "Drop confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        LoggerHelper.Logger.Debug("Sending DropCollection message, collection name:" + message.Content.Name);
                        Messenger.Default.Send(new NotificationMessage<MongoDbCollectionViewModel>(this, message.Content.Database, message.Content, Constants.DropCollectionMessage));
                    }
                    break;
                case Constants.OpenCreateIndexMessage:
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
                case Constants.ConfirmDropIndexMessage:
                    var result = MessageBox.Show("Drop index " + message.Content.Name + "?", "Drop confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        LoggerHelper.Logger.Debug("Sending DropIndex message, index name:" + message.Content.Name);
                        Messenger.Default.Send(new NotificationMessage<MongoDbIndexViewModel>(this, message.Content.Collection, message.Content, Constants.DropIndexMessage));
                    }
                    break;
                case Constants.EditIndexMessage:
                    CreateIndexDialog wnd = new CreateIndexDialog();
                    wnd.Title = "Edit index";
                    var vm = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<CreateIndexViewModel>();
                    vm.Collection = message.Content.Collection;
                    vm.Name = message.Content.Name;
                    vm.IsNew = false;
                    vm.IsExpanded = true;
                    vm.IndexDefinition = message.Content.Index["key"].ToJson(new MongoDB.Bson.IO.JsonWriterSettings() { Indent = true });
                    if (message.Content.Index.Contains("unique"))
                        vm.Unique = message.Content.Index["unique"].AsBoolean;

                    if (message.Content.Index.Contains("sparse"))
                        vm.Sparse = message.Content.Index["sparse"].AsBoolean;

                    if (message.Content.Index.Contains("expireAfterSeconds"))
                        vm.ExpireAfter = message.Content.Index["expireAfterSeconds"].AsInt32;

                    if (message.Content.Index.Contains("v"))
                        vm.Version = message.Content.Index["v"].AsInt32;

                    if (message.Content.Index.Contains("storageEngine"))
                        vm.StorageEngine = message.Content.Index["storageEngine"].ToJson(new MongoDB.Bson.IO.JsonWriterSettings() { Indent = true });

                    if (message.Content.Index.Contains("weights"))
                        vm.Weights = message.Content.Index["weights"].ToJson(new MongoDB.Bson.IO.JsonWriterSettings() { Indent = true });

                    if (message.Content.Index.Contains("default_language"))
                        vm.DefaultLanguage = message.Content.Index["default_language"].AsString;

                    if (message.Content.Index.Contains("language_override"))
                        vm.LanguageOverride = message.Content.Index["language_override"].AsString;

                    if (message.Content.Index.Contains("textIndexVersion"))
                        vm.TextIndexVersion = message.Content.Index["textIndexVersion"].AsInt32;

                    if (message.Content.Index.Contains("2dsphereIndexVersion"))
                        vm.SphereIndexVersion = message.Content.Index["2dsphereIndexVersion"].AsInt32;

                    if (message.Content.Index.Contains("bits"))
                        vm.Bits = message.Content.Index["bits"].AsInt32;

                    if (message.Content.Index.Contains("min"))
                        vm.Min = message.Content.Index["min"].AsInt32;

                    if (message.Content.Index.Contains("max"))
                        vm.Max = message.Content.Index["max"].AsInt32;

                    if (message.Content.Index.Contains("bucketSize"))
                        vm.BucketSize = message.Content.Index["bucketSize"].AsInt32;

                    wnd.DataContext = vm;
                    wnd.ShowDialog();
                    break;
            }
        }

        private void DatabaseMessageHandler(NotificationMessage<MongoDbDatabaseViewModel> message)
        {
            switch (message.Notification)
            {
                case Constants.OpenCreateNewCollectionMessage:
                    CreateCollectionDialog wnd = new CreateCollectionDialog();
                    var vm = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstanceWithoutCaching<CreateCollectionViewModel>();
                    vm.Database = message.Content;
                    wnd.DataContext = vm;
                    wnd.ShowDialog();
                    break;
                case Constants.ConfirmDropDatabaseMessage:
                    var result = MessageBox.Show("Drop database " + message.Content.Name + "?", "Drop confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Messenger.Default.Send(new NotificationMessage<MongoDbDatabaseViewModel>(this, message.Content.Server, message.Content, Constants.DropDatabaseMessage));
                    }
                    break;
            }
        }
    }
}
