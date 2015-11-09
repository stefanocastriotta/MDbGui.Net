using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MDbGui.Net.Utils;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDbGui.Net.ViewModel.Operations
{
    public class MongoDbFindOperationViewModel : MongoDbOperationViewModel
    {
        public MongoDbFindOperationViewModel(TabViewModel owner) : base(owner)
        {
            Name = "Find / Count";
            ExecuteFind = new RelayCommand<bool>((explain) =>
            {
                Skip = 0;
                InnerExecuteFind(explain);
            });

            DoPaging = new RelayCommand<bool>(InnerExecuteFind);
            PageBack = new RelayCommand(InnerPageBack);
            PageForward = new RelayCommand(InnerPageForward);
            ExecuteCount = new RelayCommand(InnerExecuteCount);
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

        public RelayCommand<bool> ExecuteFind { get; set; }

        public async void InnerExecuteFind(bool explain = false)
        {
            Owner.Executing = true;
            Guid operationID = Guid.NewGuid();
            Task<List<BsonDocument>> task = null;
            bool stopRequested = false;
            try
            {
                task = Owner.Service.FindAsync(Owner.Database, Owner.Collection, Find.Deserialize<BsonDocument>("Find"), Sort.Deserialize<BsonDocument>("Sort"), Projection.Deserialize<BsonDocument>("Projection"), Size, Skip, explain, operationID, Owner.Cts.Token);
                var results = await task.WithCancellation(Owner.Cts.Token);
                Owner.Executing = false;

                if (!explain)
                {
                    Owner.ShowPager = true;
                    StringBuilder sb = new StringBuilder();
                    int index = 1;
                    sb.Append("[");
                    foreach (var result in results)
                    {
                        sb.AppendLine();
                        sb.Append("/* # ");
                        sb.Append(index.ToString());
                        sb.AppendLine(" */");
                        sb.Append(result.ToJson(Options.JsonWriterSettings));
                        sb.Append(",");
                        index++;
                    }
                    if (results.Count > 0)
                        sb.Length -= 1;
                    sb.AppendLine();
                    sb.Append("]");

                    Owner.RawResult = sb.ToString();
                    sb.Clear();
                }
                else
                {
                    Owner.ShowPager = false;
                    if (results.Count > 0)
                        Owner.RawResult = results[0].ToJson(Options.JsonWriterSettings);
                    else
                        Owner.RawResult = "";
                }
                Owner.SelectedViewIndex = 0;

                Owner.Root = new ResultsViewModel(results, Owner);
                GC.Collect();
            }
            catch (BsonExtensions.BsonParseException ex)
            {
                LoggerHelper.Logger.Error("Exception while executing find command", ex);
                Owner.SelectedViewIndex = 1;
                Owner.RawResult = ex.Message;
                Owner.Root = null;
                Messenger.Default.Send(new NotificationMessage<BsonExtensions.BsonParseException>(this, ex, "FindParseException"));
            }
            catch (OperationCanceledException)
            {
                stopRequested = true;
            }
            catch (Exception ex)
            {
                Utils.LoggerHelper.Logger.Error("Exception while executing find command", ex);
                Owner.SelectedViewIndex = 1;
                Owner.RawResult = ex.Message;
                Owner.Root = null;
            }
            finally
            {
                Owner.Executing = false;
            }
            if (stopRequested)
            {
                if (!task.IsCompleted)
                {
                    task.ContinueWith(t =>
                    {
                        if (t.Exception != null)
                        {
                            Utils.LoggerHelper.Logger.Warn("Exception while executing find command", t.Exception);
                        }
                    });
                    var currentOp = await Owner.Service.Eval(Owner.Database, "function() { return db.currentOP(); }");
                    if (currentOp != null)
                    {
                        var operation = currentOp.AsBsonDocument["inprog"].AsBsonArray.FirstOrDefault(item => item.AsBsonDocument.Contains("query") && item.AsBsonDocument["query"].AsBsonDocument.Contains("$comment") && item.AsBsonDocument["query"]["$comment"].AsString == operationID.ToString());
                        if (operation != null)
                        {
                            await Owner.Service.Eval(Owner.Database, string.Format("function() {{ return db.killOp({0}); }}", operation["opid"].AsInt32));
                        }
                    }
                }
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

        public RelayCommand<bool> DoPaging { get; set; }

        #region Count

        public RelayCommand ExecuteCount { get; set; }

        public async void InnerExecuteCount()
        {
            Owner.Executing = true;
            try
            {
                var result = await Owner.Service.CountAsync(Owner.Database, Owner.Collection, Find.Deserialize<BsonDocument>("Find"), Owner.Cts.Token);
                Owner.Executing = false;
                Owner.ShowPager = false;

                Owner.RawResult = result.ToString();
                Owner.SelectedViewIndex = 1;

                Owner.Root = null;
            }
            catch (BsonExtensions.BsonParseException ex)
            {
                LoggerHelper.Logger.Error("Exception while executing Count command", ex);
                Owner.SelectedViewIndex = 1;
                Owner.RawResult = ex.Message;
                Owner.Root = null;
                Messenger.Default.Send(new NotificationMessage<BsonExtensions.BsonParseException>(this, ex, "FindParseException"));
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error("Exception while executing Count command", ex);
                Owner.SelectedViewIndex = 1;
                Owner.RawResult = ex.Message;
                Owner.Root = null;
            }
            finally
            {
                Owner.Executing = false;
            }
        }

        #endregion

    }
}
