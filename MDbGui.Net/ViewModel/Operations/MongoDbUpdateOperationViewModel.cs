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
    public class MongoDbUpdateOperationViewModel : MongoDbOperationViewModel
    {
        public MongoDbUpdateOperationViewModel(TabViewModel owner) : base(owner)
        {
            Name = Constants.UpdateOperation;
            DisplayName = "Update";
            ExecuteUpdate = new RelayCommand(InnerExecuteUpdate);
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

        public RelayCommand ExecuteUpdate { get; set; }

        public async void InnerExecuteUpdate()
        {
            Owner.Executing = true;
            try
            {
                var result = await Owner.Service.UpdateAsync(Owner.Database, Owner.Collection, UpdateFilter.Deserialize<BsonDocument>(Constants.UpdateFilterProperty), UpdateDocument.Deserialize<BsonDocument>(Constants.UpdateDocumentProperty), UpdateMulti, Owner.Cts.Token);

                Owner.RawResult = result.ToJson(Options.JsonWriterSettings);
                Owner.RawResult += Environment.NewLine;
                Owner.RawResult += Environment.NewLine;
                Owner.RawResult += "Modified Count: " + result.ModifiedCount;

                Owner.SelectedViewIndex = 1;
                Owner.Root = null;
            }
            catch (BsonExtensions.BsonParseException ex)
            {
                LoggerHelper.Logger.Error("Exception while executing Update command", ex);
                Owner.SelectedViewIndex = 1;
                Owner.RawResult = ex.Message;
                Owner.Root = null;
                Messenger.Default.Send(new NotificationMessage<BsonExtensions.BsonParseException>(this, ex, Constants.UpdateParseException));
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error("Exception while executing Update command", ex);
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
