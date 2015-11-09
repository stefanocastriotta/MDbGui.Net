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
    public class MongoDbReplaceOperationViewModel : MongoDbOperationViewModel
    {
        public MongoDbReplaceOperationViewModel(TabViewModel owner) : base(owner)
        {
            Name = "Replace";
            DisplayName = "Replace";
            ExecuteReplace = new RelayCommand(InnerExecuteReplace);
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

        public RelayCommand ExecuteReplace { get; set; }

        public async void InnerExecuteReplace()
        {
            Owner.Executing = true;
            try
            {
                var result = await Owner.Service.ReplaceOneAsync(Owner.Database, Owner.Collection, ReplaceFilter.Deserialize<BsonDocument>("ReplaceFilter"), Replacement.Deserialize<BsonDocument>("Replacement"), Owner.Cts.Token);

                Owner.RawResult = result.ToJson(Options.JsonWriterSettings);
                Owner.RawResult += Environment.NewLine;
                Owner.RawResult += Environment.NewLine;
                Owner.RawResult += "Modified Count: " + result.ModifiedCount;

                Owner.SelectedViewIndex = 1;
                Owner.Root = null;
            }
            catch (BsonExtensions.BsonParseException ex)
            {
                LoggerHelper.Logger.Error("Exception while executing Replace command", ex);
                Owner.SelectedViewIndex = 1;
                Owner.RawResult = ex.Message;
                Owner.Root = null;
                Messenger.Default.Send(new NotificationMessage<BsonExtensions.BsonParseException>(this, ex, "ReplaceParseException"));
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error("Exception while executing Replace command", ex);
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
