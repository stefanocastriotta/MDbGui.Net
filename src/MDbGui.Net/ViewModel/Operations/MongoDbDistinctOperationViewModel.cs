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
    public class MongoDbDistinctOperationViewModel : MongoDbOperationViewModel
    {
        public MongoDbDistinctOperationViewModel(TabViewModel owner) : base(owner)
        {
            Name = Constants.DistinctOperation;
            DisplayName = "Distinct";
            ExecuteDistinct = new RelayCommand(InnerExecuteDistinct);
        }

        private string _distinctFilter = "{}";

        public string DistinctFilter
        {
            get
            {
                return _distinctFilter;
            }
            set
            {
                Set(ref _distinctFilter, value);
            }
        }

        private string _distinctFieldName;

        public string DistinctFieldName
        {
            get
            {
                return _distinctFieldName;
            }
            set
            {
                Set(ref _distinctFieldName, value);
            }
        }

        public RelayCommand ExecuteDistinct { get; set; }

        public async void InnerExecuteDistinct()
        {
            Owner.Executing = true;
            try
            {
                var results = await Owner.Service.DistinctAsync(Owner.Database, Owner.Collection, DistinctFieldName, DistinctFilter.Deserialize<BsonDocument>(Constants.DistinctFilterProperty), Owner.Cts.Token);
                Owner.Executing = false;
                Owner.ShowPager = false;

                Owner.RawResult = results.ToJson(Options.JsonWriterSettings);
                Owner.SelectedViewIndex = 0;

                Owner.Root = new ResultsViewModel(results.Select(r => new BsonDocument(results.IndexOf(r).ToString(), r)).ToList(), Owner);
                GC.Collect();
            }
            catch (BsonExtensions.BsonParseException ex)
            {
                LoggerHelper.Logger.Error("Exception while executing Distinct command", ex);
                Owner.SelectedViewIndex = 1;
                Owner.RawResult = ex.Message;
                Owner.Root = null;
                Messenger.Default.Send(new NotificationMessage<BsonExtensions.BsonParseException>(this, ex, Constants.DistinctParseException));
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error("Exception while executing Distinct command", ex);
                Owner.SelectedViewIndex = 1;
                Owner.RawResult = ex.Message;
                Owner.Root = null;
            }
            finally
            {
                Owner.Executing = false;
            }
        }
    }
}
