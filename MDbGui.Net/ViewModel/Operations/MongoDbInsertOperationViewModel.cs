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
    public class MongoDbInsertOperationViewModel : MongoDbOperationViewModel
    {
        public MongoDbInsertOperationViewModel(TabViewModel owner) : base(owner)
        {
            Name = Constants.InsertOperation;
            DisplayName = "Insert";
            ExecuteInsert = new RelayCommand(InnerExecuteInsert);
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
            Owner.Executing = true;
            try
            {
                var result = await Owner.Service.InsertAsync(Owner.Database, Owner.Collection, Insert.Deserialize<BsonArray>(Constants.InsertProperty), Owner.Cts.Token);

                Owner.RawResult = result.ToJson(Options.JsonWriterSettings);
                Owner.RawResult += Environment.NewLine;
                Owner.RawResult += Environment.NewLine;
                Owner.RawResult += "Inserted Count: " + result.InsertedCount;
                Owner.RawResult += Environment.NewLine;
                Owner.RawResult += Environment.NewLine;
                if (result.ProcessedRequests != null && result.ProcessedRequests.Count > 0)
                    Owner.RawResult += "Processed requests: " + Environment.NewLine + string.Join(Environment.NewLine, result.ProcessedRequests.Cast<InsertOneModel<BsonDocument>>().Select(s => s.Document.ToJson(Options.JsonWriterSettings)));

                Owner.SelectedViewIndex = 1;
                Owner.Root = null;
            }
            catch (BsonExtensions.BsonParseException ex)
            {
                LoggerHelper.Logger.Error("Exception while executing Insert command", ex);
                Owner.SelectedViewIndex = 1;
                Owner.RawResult = ex.Message;
                Owner.Root = null;
                Messenger.Default.Send(new NotificationMessage<BsonExtensions.BsonParseException>(this, ex, Constants.InsertParseException));
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error("Exception while executing Insert command", ex);
                Owner.RawResult = ex.Message;
                Owner.SelectedViewIndex = 1;
                Owner.Root = null;
            }
            finally
            {
                Owner.Executing = false;
                Owner.ShowPager = false;
            }
        }
    }
}
