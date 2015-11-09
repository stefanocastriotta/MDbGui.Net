using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using MDbGui.Net.Model;
using MDbGui.Net.Utils;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading;
using MongoDB.Driver;
using MDbGui.Net.ViewModel.Operations;

namespace MDbGui.Net.ViewModel
{
    public class TabViewModel : ViewModelBase, IDisposable
    {
        public CancellationTokenSource Cts = new CancellationTokenSource();

        private bool _disposed;

        public TabViewModel()
        {
            ExecutingTimer.Tick += ExecutingTimer_Tick;
            ExecutingTimer.Interval = TimeSpan.FromMilliseconds(100);

            Connections = new List<ActiveConnection>();

            FindOperation = new MongoDbFindOperationViewModel(this);
            InsertOperation = new MongoDbInsertOperationViewModel(this);
            UpdateOperation = new MongoDbUpdateOperationViewModel(this);
            ReplaceOperation = new MongoDbReplaceOperationViewModel(this);
            RemoveOperation = new MongoDbRemoveOperationViewModel(this);
            AggregateOperation = new MongoDbAggregateOperationViewModel(this);
            DistinctOperation = new MongoDbDistinctOperationViewModel(this);
            CommandOperation = new MongoDbCommandOperationViewModel(this);
            EvalOperation = new MongoDbEvalOperationViewModel(this);

            Operations = new List<MongoDbOperationViewModel>();
            Operations.Add(FindOperation);
            Operations.Add(InsertOperation);
            Operations.Add(UpdateOperation);
            Operations.Add(ReplaceOperation);
            Operations.Add(RemoveOperation);
            Operations.Add(AggregateOperation);
            Operations.Add(DistinctOperation);
            Operations.Add(CommandOperation);
            Operations.Add(EvalOperation);

            CopyToClipboard = new RelayCommand(() =>
            {
                Clipboard.SetText(RawResult);
            });

            ExecuteStop = new RelayCommand(() =>
            {
                Cts.Cancel();
            });

            ExecuteClose = new RelayCommand(InnerExecuteClose);

            Messenger.Default.Register<NotificationMessage<ReplaceOneViewModel>>(this, (message) => ReplaceOneHandler(message));
            Messenger.Default.Register<NotificationMessage<DocumentResultViewModel>>(this, (message) => DocumentMessageHandler(message));
        }

        public List<MongoDbOperationViewModel> Operations { get; private set; }

        private MongoDbOperationViewModel _selectedOperation;

        public MongoDbOperationViewModel SelectedOperation
        {
            get
            {
                return _selectedOperation;
            }
            set
            {
                Set(ref _selectedOperation, value);
                if (value != null && value.Name == "Find")
                {
                    if (Root != null && Root.Children.Count > 0)
                        ShowPager = true;
                }
                else
                    ShowPager = false;
            }
        }

        public MongoDbFindOperationViewModel FindOperation { get;set; }
        public MongoDbInsertOperationViewModel InsertOperation { get; set; }
        public MongoDbUpdateOperationViewModel UpdateOperation { get; set; }
        public MongoDbReplaceOperationViewModel ReplaceOperation { get; set; }
        public MongoDbRemoveOperationViewModel RemoveOperation { get; set; }
        public MongoDbAggregateOperationViewModel AggregateOperation { get; set; }
        public MongoDbDistinctOperationViewModel DistinctOperation { get; set; }
        public MongoDbCommandOperationViewModel CommandOperation { get; set; }
        public MongoDbEvalOperationViewModel EvalOperation { get; set; }

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

        private bool _executing;

        public bool Executing
        {
            get
            {
                return _executing;
            }
            set
            {
                ShowProgress = true;
                if (value && !_executing)
                {
                    Cts = new CancellationTokenSource();
                    ExecutingTime = TimeSpan.Zero.ToString("hh':'mm':'ss'.'fff");
                    _executingStartTime = DateTime.Now;
                    ExecutingTimer.Start();
                }
                else if (!value && _executing)
                {
                    ExecutingTimer.Stop();
                }
                Set(ref _executing, value);
            }
        }

        private DispatcherTimer ExecutingTimer = new DispatcherTimer();

        private DateTime _executingStartTime;

        private string _executingTime = string.Empty;

        public string ExecutingTime
        {
            get
            {
                return _executingTime;
            }
            set
            {
                Set(ref _executingTime, value);
            }
        }

        private bool _executeOnOpen;

        public bool ExecuteOnOpen
        {
            get
            {
                return _executeOnOpen;
            }
            set
            {
                Set(ref _executeOnOpen, value);
            }
        }

        private bool _showProgress;

        public bool ShowProgress
        {
            get
            {
                return _showProgress;
            }
            set
            {
                Set(ref _showProgress, value);
            }
        }

        private bool _showPager;

        public bool ShowPager
        {
            get
            {
                return _showPager;
            }
            set
            {
                Set(ref _showPager, value);
            }
        }


        private string _rawResult = null;

        public string RawResult
        {
            get
            {
                return _rawResult;
            }
            set
            {
                Set(ref _rawResult, value);
            }
        }

        private int _selectedViewIndex = 0;

        public int SelectedViewIndex
        {
            get
            {
                return _selectedViewIndex;
            }
            set
            {
                Set(ref _selectedViewIndex, value);
            }
        }


        private ResultsViewModel _root;

        public ResultsViewModel Root
        {
            get
            {
                return _root;
            }
            set
            {
                Set(ref _root, value);
            }
        }

        void ExecutingTimer_Tick(object sender, System.EventArgs e)
        {
            ExecutingTime = (DateTime.Now - _executingStartTime).ToString("hh':'mm':'ss'.'fff");
        }


        private List<ActiveConnection> _connections;
        public List<ActiveConnection> Connections
        {
            get { return _connections; }
            set
            {
                Set(ref _connections, value);
            }
        }


        private IMongoDbService _service;
        public IMongoDbService Service
        {
            get { return _service; }
            set
            {
                Set(ref _service, value);
            }
        }

        private string _database = string.Empty;

        public string Database
        {
            get
            {
                return _database;
            }
            set
            {
                Set(ref _database, value);
            }
        }

        private string _collection = string.Empty;

        public string Collection
        {
            get
            {
                return _collection;
            }
            set
            {
                Set(ref _collection, value);
            }
        }

        public RelayCommand ExecuteClose { get; set; }

        public void InnerExecuteClose()
        {
            Messenger.Default.Send(new NotificationMessage<TabViewModel>(this, "CloseTab"));
        }

        public RelayCommand ExecuteStop { get; set; }

        public RelayCommand CopyToClipboard { get; set; }

        private async void ReplaceOneHandler(NotificationMessage<ReplaceOneViewModel> message)
        {
            if (message.Target == this && message.Notification == "UpdateDocument")
            {
                Executing = true;
                try
                {
                    var result = await Service.ReplaceOneAsync(Database, Collection, BsonDocument.Parse("{ _id: ObjectId('" + message.Content.Document.Id + "') }"), message.Content.ReplacementBsonDocument, Cts.Token);

                    var doc = message.Content.Replacement.Deserialize<BsonDocument>();
                    message.Content.Document.Result = doc;
                    message.Content.Document.IsExpanded = false;
                    message.Content.Document.Value = "(" + doc.ElementCount + ") fields";
                    message.Content.Document.Children.Clear();
                    message.Content.Document.LazyLoading = true;
                }
                catch (Exception ex)
                {
                    LoggerHelper.Logger.Error("Exception while updating a document", ex);
                }
                finally
                {
                    Executing = false;
                }
            }
        }


        private async void DocumentMessageHandler(NotificationMessage<DocumentResultViewModel> message)
        {
            if (message.Notification == "DeleteResult")
            {
                Executing = true;
                try
                {
                    var result = await Service.DeleteAsync(Database, Collection, ("{_id: ObjectId('" + message.Content.Id + "')}").Deserialize<BsonDocument>("Id"), true, Cts.Token);
                    if (result.DeletedCount == 1 && Root != null)
                    {
                        Root.Children.Remove(message.Content);
                        StringBuilder sb = new StringBuilder();
                        sb.Append("[");
                        foreach (var item in Root.Children)
                        {
                            sb.AppendLine();
                            sb.Append("/* # ");
                            sb.Append(((DocumentResultViewModel)item).Index);
                            sb.AppendLine(" */");
                            sb.AppendLine(((DocumentResultViewModel)item).Result.ToJson(Options.JsonWriterSettings));
                            sb.Append(",");
                        }
                        if (Root.Children.Count > 0)
                            sb.Length -= 1;
                        sb.AppendLine();
                        sb.Append("]");
                        RawResult = sb.ToString();
                        sb.Clear();
                        GC.Collect();
                    }
                    else
                        RawResult = "[" + Environment.NewLine + "\t" + Environment.NewLine + "]";
                }
                catch (Exception ex)
                {
                    LoggerHelper.Logger.Error("Exception while deleting a document", ex);
                    RawResult = ex.Message;
                    SelectedViewIndex = 1;
                }
                finally
                {
                    Executing = false;
                }
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            if (Root != null)
            {
                foreach (var child in Root.Children)
                    child.Children.Clear();
                Root.Children.Clear();
            }
            Root = null;
            RawResult = null;
            MessengerInstance.Unregister(this);
            GC.Collect();
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
                Cts.Dispose();

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }
}
