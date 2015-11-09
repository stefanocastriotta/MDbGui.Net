using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MDbGui.Net.Utils;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDbGui.Net.ViewModel.Operations
{
    public class MongoDbAggregateOperationViewModel : MongoDbOperationViewModel
    {
        public MongoDbAggregateOperationViewModel(TabViewModel owner) : base(owner)
        {
            Name = "Aggregate";
            DisplayName = "Aggregate";
            AggregateOptions = new AggregateOptions();
            ExecuteAggregate = new RelayCommand(InnerExecuteAggregate);
        }

        private string _aggregatePipeline = "[" + Environment.NewLine + "\t" + Environment.NewLine + "]";

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
            Owner.Executing = true;
            Guid operationID = Guid.NewGuid();
            Task<List<BsonDocument>> task = null;
            bool stopRequested = false;
            try
            {
                var pipeline = AggregatePipeline.Deserialize<BsonArray>();
                task = Owner.Service.AggregateAsync(Owner.Database, Owner.Collection, AggregatePipeline.Deserialize<BsonArray>("AggregatePipeline"), AggregateOptions, AggregateExplain, Owner.Cts.Token);
                var results = await task.WithCancellation(Owner.Cts.Token);
                Owner.Executing = false;
                Owner.ShowPager = false;
                StringBuilder sb = new StringBuilder();
                int index = 1;
                sb.Append("[");
                foreach (var result in results)
                {
                    sb.AppendLine();
                    sb.Append("/* # ");
                    sb.Append(index.ToString());
                    sb.AppendLine(" */");
                    sb.AppendLine(result.ToJson(Options.JsonWriterSettings));
                    sb.Append(",");
                    index++;
                }
                if (results.Count > 0)
                    sb.Length -= 1;
                sb.AppendLine();
                sb.Append("]");

                Owner.RawResult = sb.ToString();
                Owner.SelectedViewIndex = 0;
                sb.Clear();
                Owner.Root = new ResultsViewModel(results, Owner);
                GC.Collect();
            }
            catch (OperationCanceledException)
            {
                stopRequested = true;
            }
            catch (BsonExtensions.BsonParseException ex)
            {
                LoggerHelper.Logger.Error("Exception while executing Aggregate command", ex);
                Owner.SelectedViewIndex = 1;
                Owner.RawResult = ex.Message;
                Owner.Root = null;
                Messenger.Default.Send(new NotificationMessage<BsonExtensions.BsonParseException>(this, ex, "AggregateParseException"));
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error("Exception while executing Aggregate command", ex);
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
                            LoggerHelper.Logger.Warn("Exception while executing find command", t.Exception);
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
    }
}
