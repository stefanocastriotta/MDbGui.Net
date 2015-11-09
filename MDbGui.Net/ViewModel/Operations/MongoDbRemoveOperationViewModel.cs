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
    public class MongoDbRemoveOperationViewModel : MongoDbOperationViewModel
    {
        public MongoDbRemoveOperationViewModel(TabViewModel owner) : base(owner)
        {
            Name = "Remove";
            DisplayName = "Remove";
            ExecuteDelete = new RelayCommand(InnerExecuteDelete);
        }

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
            Owner.Executing = true;
            try
            {
                var result = await Owner.Service.DeleteAsync(Owner.Database, Owner.Collection, DeleteQuery.Deserialize<BsonDocument>("DeleteQuery"), DeleteJustOne, Owner.Cts.Token);

                Owner.RawResult = result.ToJson(Options.JsonWriterSettings);
                Owner.RawResult += Environment.NewLine;
                Owner.RawResult += Environment.NewLine;
                Owner.RawResult += "Deleted Count: " + result.DeletedCount;

                Owner.SelectedViewIndex = 1;
                Owner.Root = null;
            }
            catch (BsonExtensions.BsonParseException ex)
            {
                LoggerHelper.Logger.Error("Exception while executing Delete command", ex);
                Owner.SelectedViewIndex = 1;
                Owner.RawResult = ex.Message;
                Owner.Root = null;
                Messenger.Default.Send(new NotificationMessage<BsonExtensions.BsonParseException>(this, ex, "DeleteParseException"));
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error("Exception while executing Delete command", ex);
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
