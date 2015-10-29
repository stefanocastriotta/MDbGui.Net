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

namespace MDbGui.Net.ViewModel
{
    public class TabViewModel : ViewModelBase, IDisposable
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        private bool _disposed;

        public TabViewModel()
        {
            ExecutingTimer.Tick += ExecutingTimer_Tick;
            ExecutingTimer.Interval = TimeSpan.FromMilliseconds(100);

            Connections = new List<ActiveConnection>();

            CommandTypes = new Dictionary<CommandType, string>();
            CommandTypes.Add(Model.CommandType.Find, "Find / Count");
            CommandTypes.Add(Model.CommandType.Insert, "Insert");
            CommandTypes.Add(Model.CommandType.Update, "Update");
            CommandTypes.Add(Model.CommandType.Replace, "Replace");
            CommandTypes.Add(Model.CommandType.Remove, "Remove");
            CommandTypes.Add(Model.CommandType.Aggregate, "Aggregate");
            CommandTypes.Add(Model.CommandType.RunCommand, "RunCommand");
            CommandTypes.Add(Model.CommandType.Eval, "Eval");

            AggregateOptions = new MongoDB.Driver.AggregateOptions();

            CopyToClipboard = new RelayCommand(() =>
            {
                Clipboard.SetText(RawResult);
            });

            ExecuteStop = new RelayCommand(() =>
            {
                cts.Cancel();
            });

            ExecuteClose = new RelayCommand(InnerExecuteClose);

            ExecuteFind = new RelayCommand(() =>
            {
                Skip = 0;
                InnerExecuteFind();
            });

            ExecuteFindExplain = new RelayCommand(InnerExecuteFindExplain);

            DoPaging = new RelayCommand(InnerExecuteFind);
            PageBack = new RelayCommand(InnerPageBack);
            PageForward = new RelayCommand(InnerPageForward);

            ExecuteCount = new RelayCommand(InnerExecuteCount);

            ExecuteCommand = new RelayCommand(InnerExecuteCommand);

            ExecuteEval = new RelayCommand(InnerExecuteEval);

            ExecuteInsert = new RelayCommand(InnerExecuteInsert);

            ExecuteUpdate = new RelayCommand(InnerExecuteUpdate);
            
            ExecuteReplace = new RelayCommand(InnerExecuteReplace);

            ExecuteDelete = new RelayCommand(InnerExecuteDelete);

            ExecuteAggregate = new RelayCommand(InnerExecuteAggregate);

            Messenger.Default.Register<NotificationMessage<DocumentResultViewModel>>(this, (message) => DocumentMessageHandler(message));
        }

        public Dictionary<CommandType, string> CommandTypes { get; private set; }
 
        private CommandType _commandType;

        public CommandType CommandType
        {
            get
            {
                return _commandType;
            }
            set
            {
                Set(ref _commandType, value);
            }
        }


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
                    cts = new CancellationTokenSource();
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

        #region Find

        private string _find = "{}";

        public string Find
        {
            get
            {
                return _find;
            }
            set
            {
                Set(ref _find, value);
            }
        }

        private string _sort = "{}";

        public string Sort
        {
            get
            {
                return _sort;
            }
            set
            {
                Set(ref _sort, value);
            }
        }

        private string _projection = "{}";

        public string Projection
        {
            get
            {
                return _projection;
            }
            set
            {
                Set(ref _projection, value);
            }
        }

        private int _skip = 0;

        public int Skip
        {
            get
            {
                return _skip;
            }
            set
            {
                Set(ref _skip, value);
            }
        }

        private int _size = 50;

        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                Set(ref _size, value);
            }
        }

        public RelayCommand ExecuteFind { get; set; }

        public async void InnerExecuteFind()
        {
            Executing = true;
            Guid operationID = Guid.NewGuid();
            var task = Service.FindAsync(Database, Collection, Find, Sort, Projection, Size, Skip, false, operationID, cts.Token);
            try
            {
                var results = await task.WithCancellation(cts.Token);
                Executing = false;
                ShowPager = true;
                StringBuilder sb = new StringBuilder();
                int index = 1;
                sb.Append("[");
                var serializer = MongoDB.Bson.Serialization.BsonSerializer.LookupSerializer(typeof(BsonDocument));
                MongoDB.Bson.Serialization.BsonSerializationArgs args = default(MongoDB.Bson.Serialization.BsonSerializationArgs);
                foreach (var result in results)
                {
                    sb.AppendLine();
                    sb.Append("/* # ");
                    sb.Append(index.ToString());
                    sb.AppendLine(" */");

                    using (var stringWriter = new System.IO.StringWriter())
                    {
                        using (var bsonWriter = new LocalDateTimeJsonWriter(stringWriter, new JsonWriterSettings { Indent = true }))
                        {
                            var context = MongoDB.Bson.Serialization.BsonSerializationContext.CreateRoot(bsonWriter, null);
                            args.NominalType = typeof(BsonDocument);
                            serializer.Serialize(context, args, result);
                        }
                        sb.Append(stringWriter.ToString());
                    }

                    sb.Append(",");
                    index++;
                }
                if (results.Count > 0)
                    sb.Length -= 1;
                sb.AppendLine();
                sb.Append("]");

                RawResult = sb.ToString();
                SelectedViewIndex = 0;

                Root = new ResultsViewModel(results, this);
            }
            catch (OperationCanceledException)
            {
                if (!task.IsCompleted)
                {
                    task.ContinueWith(t =>
                    {
                        if (t.Exception != null)
                        {

                        }
                    });
                    var currentOpTask = Service.Eval(Database, "function() { return db.currentOP(); }");
                    currentOpTask.Wait();
                    var currentOp = currentOpTask.Result;
                    if (currentOp != null)
                    {
                        var operation = currentOp.AsBsonDocument["inprog"].AsBsonArray.FirstOrDefault(item => item.AsBsonDocument.Contains("query") && item.AsBsonDocument["query"].AsBsonDocument.Contains("$comment") && item.AsBsonDocument["query"]["$comment"].AsString == operationID.ToString());
                        if (operation != null)
                        {
                            var killOpTask = Service.Eval(Database, string.Format("function() {{ return db.killOp({0}); }}", operation["opid"].AsInt32));
                            killOpTask.Wait();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SelectedViewIndex = 1;
                RawResult = ex.Message;
                Root = null;
            }
            finally
            {
                Executing = false;
            }
        }

        public RelayCommand PageBack { get; set; }

        public void InnerPageBack()
        {
            Skip -= Size;
            Skip = Math.Max(0, Skip);
            InnerExecuteFind();
        }

        public RelayCommand PageForward { get; set; }

        public void InnerPageForward()
        {
            Skip += Size;
            InnerExecuteFind();
        }

        public RelayCommand DoPaging { get; set; }

        public RelayCommand ExecuteFindExplain { get; set; }

        public async void InnerExecuteFindExplain()
        {
            Executing = true;
            Guid operationID = Guid.NewGuid();
            var task = Service.FindAsync(Database, Collection, Find, Sort, Projection, Size, Skip, true, operationID, cts.Token);
            try
            {
                var results = await task.WithCancellation(cts.Token);
                Executing = false;
                ShowPager = false;

                if (results.Count > 0)
                    RawResult = results[0].ToJson(new JsonWriterSettings { Indent = true });
                else
                    RawResult = "";
                SelectedViewIndex = 0;

                Root = new ResultsViewModel(results, this);
            }
            catch (OperationCanceledException)
            {
                if (!task.IsCompleted)
                {
                    task.ContinueWith(t =>
                    {
                        if (t.Exception != null)
                        {

                        }
                    });
                    var currentOpTask = Service.Eval(Database, "function() { return db.currentOP(); }");
                    currentOpTask.Wait();
                    var currentOp = currentOpTask.Result;
                    if (currentOp != null)
                    {
                        var operation = currentOp.AsBsonDocument["inprog"].AsBsonArray.FirstOrDefault(item => item.AsBsonDocument.Contains("query") && item.AsBsonDocument["query"].AsBsonDocument.Contains("$comment") && item.AsBsonDocument["query"]["$comment"].AsString == operationID.ToString());
                        if (operation != null)
                        {
                            var killOpTask = Service.Eval(Database, string.Format("function() {{ return db.killOp({0}); }}", operation["opid"].AsInt32));
                            killOpTask.Wait();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SelectedViewIndex = 1;
                RawResult = ex.Message;
                Root = null;
            }
            finally
            {
                Executing = false;
            }
        }

        #endregion

        #region Count

        public RelayCommand ExecuteCount { get; set; }

        public async void InnerExecuteCount()
        {
            Executing = true;
            try
            {
                var result = await Service.CountAsync(Database, Collection, Find, cts.Token);
                Executing = false;
                ShowPager = false;

                RawResult = result.ToString();
                SelectedViewIndex = 1;

                Root = null;
            }
            catch (Exception ex)
            {
                SelectedViewIndex = 1;
                RawResult = ex.Message;
                Root = null;
            }
            finally
            {
                Executing = false;
            }
        }

        #endregion

        #region Command

        private string _command = "{}";

        public string Command
        {
            get
            {
                return _command;
            }
            set
            {
                Set(ref _command, value);
            }
        }

        public RelayCommand ExecuteCommand { get; set; }

        public async void InnerExecuteCommand()
        {
            Executing = true;
            try
            {
                var result = await Service.ExecuteRawCommandAsync(Database, Command, cts.Token);

                RawResult = result.ToJson(new JsonWriterSettings { Indent = true });

                SelectedViewIndex = 1;
                Root = new ResultsViewModel(new List<BsonDocument>() { result }, this);
            }
            catch (Exception ex)
            {
                RawResult = ex.Message;
                SelectedViewIndex = 1;
                Root = null;
            }
            finally
            {
                Executing = false;
                ShowPager = false;
            }
        }

        #endregion

        #region Eval

        private string _evalFunction = "function () {" + Environment.NewLine + "\t" + Environment.NewLine + "}";

        public string EvalFunction
        {
            get
            {
                return _evalFunction;
            }
            set
            {
                Set(ref _evalFunction, value);
            }
        }


        public RelayCommand ExecuteEval { get; set; }

        public async void InnerExecuteEval()
        {
            Executing = true;
            try
            {
                var result = await Service.Eval(Database, EvalFunction);

                RawResult = result.ToJson(new JsonWriterSettings { Indent = true });

                if (result.IsBsonDocument)
                    Root = new ResultsViewModel(new List<BsonDocument>() { result.AsBsonDocument }, this);
                else
                    Root = new ResultsViewModel(new List<BsonDocument>() { new BsonDocument().Add("result", result) }, this);
            }
            catch (Exception ex)
            {
                RawResult = ex.Message;
                Root = null;
            }
            finally
            {
                SelectedViewIndex = 1;
                Executing = false;
                ShowPager = false;
            }
        }

        #endregion

        #region Insert

        private string _insert = "[" + Environment.NewLine + "\t" + Environment.NewLine + "]";

        public string Insert
        {
            get
            {
                return _insert;
            }
            set
            {
                Set(ref _insert, value);
            }
        }

        public RelayCommand ExecuteInsert { get; set; }

        public async void InnerExecuteInsert()
        {
            Executing = true;
            try
            {
                BsonArray array = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(Insert);
                var result = await Service.InsertAsync(Database, Collection, array.Select(i => i.AsBsonDocument), cts.Token);

                RawResult = result.ToJson(new JsonWriterSettings { Indent = true });
                RawResult += Environment.NewLine;
                RawResult += Environment.NewLine;
                RawResult += "Inserted Count: " + result.InsertedCount;
                RawResult += Environment.NewLine;
                RawResult += Environment.NewLine;
                if (result.ProcessedRequests != null && result.ProcessedRequests.Count > 0)
                    RawResult += "Processed requests: " + Environment.NewLine + string.Join(Environment.NewLine, result.ProcessedRequests.Cast<MongoDB.Driver.InsertOneModel<BsonDocument>>().Select(s => s.Document.ToJson(new JsonWriterSettings { Indent = true })));

                SelectedViewIndex = 1;
                Root = null;
            }
            catch (Exception ex)
            {
                RawResult = ex.Message;
                SelectedViewIndex = 1;
                Root = null;
            }
            finally
            {
                Executing = false;
                ShowPager = false;
            }
        }

        #endregion

        #region Update

        private string _updateFilter = "{}";

        public string UpdateFilter
        {
            get
            {
                return _updateFilter;
            }
            set
            {
                Set(ref _updateFilter, value);
            }
        }

        private string _updateDocument = "{}";

        public string UpdateDocument
        {
            get
            {
                return _updateDocument;
            }
            set
            {
                Set(ref _updateDocument, value);
            }
        }

        private bool _updateMulti;

        public bool UpdateMulti
        {
            get
            {
                return _updateMulti;
            }
            set
            {
                Set(ref _updateMulti, value);
            }
        }

        public RelayCommand ExecuteUpdate { get; set; }

        public async void InnerExecuteUpdate()
        {
            Executing = true;
            try
            {
                var result = await Service.UpdateAsync(Database, Collection, UpdateFilter, BsonDocument.Parse(UpdateDocument), UpdateMulti, cts.Token);

                RawResult = result.ToJson(new JsonWriterSettings { Indent = true });
                RawResult += Environment.NewLine;
                RawResult += Environment.NewLine;
                RawResult += "Modified Count: " + result.ModifiedCount;

                SelectedViewIndex = 1;
                Root = null;
            }
            catch (Exception ex)
            {
                RawResult = ex.Message;
                SelectedViewIndex = 1;
                Root = null;
            }
            finally
            {
                Executing = false;
                ShowPager = false;
            }
        }

        #endregion

        #region Replace

        private string _replaceFilter = "{}";

        public string ReplaceFilter
        {
            get
            {
                return _replaceFilter;
            }
            set
            {
                Set(ref _replaceFilter, value);
            }
        }

        private string _replacement = "{}";

        public string Replacement
        {
            get
            {
                return _replacement;
            }
            set
            {
                Set(ref _replacement, value);
            }
        }

        public RelayCommand ExecuteReplace { get; set; }

        public async void InnerExecuteReplace()
        {
            Executing = true;
            try
            {
                var result = await Service.ReplaceOneAsync(Database, Collection, ReplaceFilter, BsonDocument.Parse(Replacement), cts.Token);

                RawResult = result.ToJson(new JsonWriterSettings { Indent = true });
                RawResult += Environment.NewLine;
                RawResult += Environment.NewLine;
                RawResult += "Modified Count: " + result.ModifiedCount;

                SelectedViewIndex = 1;
                Root = null;
            }
            catch (Exception ex)
            {
                RawResult = ex.Message;
                SelectedViewIndex = 1;
                Root = null;
            }
            finally
            {
                Executing = false;
                ShowPager = false;
            }
        }

        #endregion

        #region Delete

        public RelayCommand ExecuteDelete { get; set; }

        private string _deleteQuery = "{}";

        public string DeleteQuery
        {
            get
            {
                return _deleteQuery;
            }
            set
            {
                Set(ref _deleteQuery, value);
            }
        }

        private bool _deleteJustOne;

        public bool DeleteJustOne
        {
            get
            {
                return _deleteJustOne;
            }
            set
            {
                Set(ref _deleteJustOne, value);
            }
        }

        public async void InnerExecuteDelete()
        {
            Executing = true;
            try
            {
                var result = await Service.DeleteAsync(Database, Collection, DeleteQuery, DeleteJustOne, cts.Token);

                RawResult = result.ToJson(new JsonWriterSettings { Indent = true });
                RawResult += Environment.NewLine;
                RawResult += Environment.NewLine;
                RawResult += "Deleted Count: " + result.DeletedCount;

                SelectedViewIndex = 1;
                Root = null;
            }
            catch (Exception ex)
            {
                RawResult = ex.Message;
                SelectedViewIndex = 1;
                Root = null;
            }
            finally
            {
                Executing = false;
                ShowPager = false;
            }
        }

        private async void DocumentMessageHandler(NotificationMessage<DocumentResultViewModel> message)
        {
            if (message.Notification == "DeleteResult")
            {
                Executing = true;
                try
                {
                    var result = await Service.DeleteAsync(Database, Collection, "{_id: ObjectId('" + message.Content.Id + "')}", true, cts.Token);
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
                            sb.AppendLine(((DocumentResultViewModel)item).Result.ToJson(new JsonWriterSettings { Indent = true }));
                            sb.Append(",");
                        }
                        if (Root.Children.Count > 0)
                            sb.Length -= 1;
                        sb.AppendLine();
                        sb.Append("]");
                        RawResult = sb.ToString();
                    }
                    else
                        RawResult = "[" + Environment.NewLine + "\t" + Environment.NewLine + "]";
                }
                catch (Exception ex)
                {
                    RawResult = ex.Message;
                    SelectedViewIndex = 1;
                }
                finally
                {
                    Executing = false;
                }
            }
        }

        #endregion

        #region Aggregate

        private string _aggregatePipeline = "{}";

        public string AggregatePipeline
        {
            get
            {
                return _aggregatePipeline;
            }
            set
            {
                Set(ref _aggregatePipeline, value);
            }
        }

        private AggregateOptions _aggregateOptions;

        public AggregateOptions AggregateOptions
        {
            get
            {
                return _aggregateOptions;
            }
            set
            {
                Set(ref _aggregateOptions, value);
            }
        }

        private bool _aggregateExplain;

        public bool AggregateExplain
        {
            get
            {
                return _aggregateExplain;
            }
            set
            {
                Set(ref _aggregateExplain, value);
            }
        }

        public RelayCommand ExecuteAggregate { get; set; }

        public async void InnerExecuteAggregate()
        {
            Executing = true;
            Guid operationID = Guid.NewGuid();
            Task<List<BsonDocument>> task = null;
            try
            {
                task = Service.AggregateAsync(Database, Collection, AggregatePipeline, AggregateOptions, AggregateExplain, cts.Token);
                var results = await task.WithCancellation(cts.Token);
                Executing = false;
                ShowPager = false;
                StringBuilder sb = new StringBuilder();
                int index = 1;
                sb.Append("[");
                foreach (var result in results)
                {
                    sb.AppendLine();
                    sb.Append("/* # ");
                    sb.Append(index.ToString());
                    sb.AppendLine(" */");
                    sb.AppendLine(result.ToJson(new JsonWriterSettings { Indent = true }));
                    sb.Append(",");
                    index++;
                }
                if (results.Count > 0)
                    sb.Length -= 1;
                sb.AppendLine();
                sb.Append("]");

                RawResult = sb.ToString();
                SelectedViewIndex = 0;

                Root = new ResultsViewModel(results, this);
            }
            catch (OperationCanceledException)
            {
                if (!task.IsCompleted)
                {
                    task.ContinueWith(t =>
                    {
                        if (t.Exception != null)
                        {

                        }
                    });
                    var currentOpTask = Service.Eval(Database, "function() { return db.currentOP(); }");
                    currentOpTask.Wait();
                    var currentOp = currentOpTask.Result;
                    if (currentOp != null)
                    {
                        var operation = currentOp.AsBsonDocument["inprog"].AsBsonArray.FirstOrDefault(item => item.AsBsonDocument.Contains("query") && item.AsBsonDocument["query"].AsBsonDocument.Contains("$comment") && item.AsBsonDocument["query"]["$comment"].AsString == operationID.ToString());
                        if (operation != null)
                        {
                            var killOpTask = Service.Eval(Database, string.Format("function() {{ return db.killOp({0}); }}", operation["opid"].AsInt32));
                            killOpTask.Wait();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: log error
                SelectedViewIndex = 1;
                RawResult = ex.Message;
                Root = null;
            }
            finally
            {
                Executing = false;
            }
        }

        #endregion

        public override void Cleanup()
        {
            base.Cleanup();
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
