using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using MongoDbGui.Model;
using MongoDbGui.Utils;
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

namespace MongoDbGui.ViewModel
{
    public class TabViewModel : ViewModelBase
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        public TabViewModel()
        {
            ExecutingTimer.Tick += ExecutingTimer_Tick;
            ExecutingTimer.Interval = TimeSpan.FromMilliseconds(100);

            CommandTypes = new Dictionary<CommandType, string>();
            CommandTypes.Add(Model.CommandType.Find, "Find / Count");
            CommandTypes.Add(Model.CommandType.Insert, "Insert");
            CommandTypes.Add(Model.CommandType.Update, "Update");
            CommandTypes.Add(Model.CommandType.Replace, "Replace");
            CommandTypes.Add(Model.CommandType.Remove, "Remove");
            CommandTypes.Add(Model.CommandType.Aggregate, "Aggregate");
            CommandTypes.Add(Model.CommandType.RunCommand, "RunCommand");

            ExecuteFind = new RelayCommand(() =>
            {
                cts = new CancellationTokenSource();
                Skip = 0;
                InnerExecuteFind();
            });
            ExecuteStop = new RelayCommand(() =>
            {
                cts.Cancel();
            });

            DoPaging = new RelayCommand(InnerExecuteFind);
            ExecuteCount = new RelayCommand(InnerExecuteCount);
            ExecuteClose = new RelayCommand(InnerExecuteClose);
            PageBack = new RelayCommand(InnerPageBack);
            PageForward = new RelayCommand(InnerPageForward);
            CopyToClipboard = new RelayCommand(() =>
            {
                Clipboard.SetText(RawResult);
            });

            ExecuteCommand = new RelayCommand(InnerExecuteCommand);

            ExecuteEval = new RelayCommand(InnerExecuteEval);

            ExecuteInsert = new RelayCommand(InnerExecuteInsert);

            ExecuteUpdate = new RelayCommand(InnerExecuteUpdate);
            
            ExecuteReplace = new RelayCommand(InnerExecuteReplace);

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


        private MongoDbServerViewModel _server;
        public MongoDbServerViewModel Server
        {
            get { return _server; }
            set
            {
                Set(ref _server, value);
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

        public RelayCommand ExecuteClose { get; set; }

        public void InnerExecuteClose()
        {
            Messenger.Default.Send(new NotificationMessage<TabViewModel>(this, "CloseTab"));
        }

        public RelayCommand ExecuteStop { get; set; }


        public RelayCommand ExecuteFind { get; set; }

        public async void InnerExecuteFind()
        {
            Executing = true;
            Guid operationID = Guid.NewGuid();
            var task = Server.MongoDbService.FindAsync(Database, Collection, Find, Sort, Size, Skip, operationID, cts.Token);
            try
            {
                var results = await task.WithCancellation(cts.Token);
                Executing = false;
                ShowPager = true;
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
                    var currentOpTask = Server.MongoDbService.Eval(Database, "function() { return db.currentOP(); }");
                    currentOpTask.Wait();
                    var currentOp = currentOpTask.Result;
                    if (currentOp != null)
                    {
                        var operation = currentOp.AsBsonDocument["inprog"].AsBsonArray.FirstOrDefault(item => item.AsBsonDocument.Contains("query") && item.AsBsonDocument["query"].AsBsonDocument.Contains("$comment") && item.AsBsonDocument["query"]["$comment"].AsString == operationID.ToString());
                        if (operation != null)
                        {
                            var killOpTask = Server.MongoDbService.Eval(Database, string.Format("function() {{ return db.killOp({0}); }}", operation["opid"].AsInt32));
                            killOpTask.Wait();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: log error
            }
            finally
            {
                Executing = false;
            }
        }

        public RelayCommand ExecuteCount { get; set; }

        public async void InnerExecuteCount()
        {
            Executing = true;
            try
            {
                var result = await Server.MongoDbService.CountAsync(Database, Collection, Find, cts.Token);
                Executing = false;
                ShowPager = false;

                RawResult = result.ToString();
                SelectedViewIndex = 1;

                Root = null;
            }
            catch (Exception ex)
            {
                //TODO: log error
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

        public RelayCommand CopyToClipboard { get; set; }


        public RelayCommand ExecuteCommand { get; set; }

        public async void InnerExecuteCommand()
        {
            Executing = true;
            try
            {
                var result = await Server.MongoDbService.ExecuteRawCommandAsync(Database, Command, cts.Token);

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

        public RelayCommand ExecuteEval { get; set; }

        public async void InnerExecuteEval()
        {
            Executing = true;
            try
            {
                var result = await Server.MongoDbService.Eval(Database, Command);

                RawResult = result.ToJson(new JsonWriterSettings { Indent = true });

            }
            catch (Exception ex)
            {
                RawResult = ex.Message;
            }
            finally
            {
                SelectedViewIndex = 1;
                Root = null;
                Executing = false;
                ShowPager = false;
            }
        }


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
                var result = await Server.MongoDbService.InsertAsync(Database, Collection, array.Select(i => i.AsBsonDocument), cts.Token);

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

        public RelayCommand ExecuteUpdate { get; set; }

        public async void InnerExecuteUpdate()
        {
            Executing = true;
            try
            {
                var result = await Server.MongoDbService.UpdateAsync(Database, Collection, UpdateFilter, BsonDocument.Parse(UpdateDocument), UpdateMulti, cts.Token);

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

        public RelayCommand ExecuteReplace { get; set; }

        public async void InnerExecuteReplace()
        {
            Executing = true;
            try
            {
                var result = await Server.MongoDbService.ReplaceOneAsync(Database, Collection, ReplaceFilter, BsonDocument.Parse(Replacement), cts.Token);

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

        private async void DocumentMessageHandler(NotificationMessage<DocumentResultViewModel> message)
        {
            if (message.Notification == "DeleteResult")
            {
                Executing = true;
                try
                {
                    var result = await Server.MongoDbService.DeleteOneAsync(Database, Collection, "{_id: ObjectId('" + message.Content.Id + "')}", cts.Token);
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
                }
                finally
                {
                    Executing = false;
                }
            }
        }

    }
}
