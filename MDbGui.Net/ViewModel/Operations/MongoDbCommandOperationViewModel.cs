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
    public class MongoDbCommandOperationViewModel : MongoDbOperationViewModel
    {
        public MongoDbCommandOperationViewModel(TabViewModel owner) : base(owner)
        {
            Name = Constants.CommandOperation;
            DisplayName = "Run command";
            ExecuteCommand = new RelayCommand(InnerExecuteCommand);
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

        public RelayCommand ExecuteCommand { get; set; }

        public async void InnerExecuteCommand()
        {
            Owner.Executing = true;
            try
            {
                var result = await Owner.Service.ExecuteRawCommandAsync(Owner.Database, Command.Deserialize<BsonDocument>(Constants.CommandPropertyProperty), Owner.Cts.Token);

                Owner.RawResult = result.ToJson(Options.JsonWriterSettings);

                Owner.SelectedViewIndex = 1;
                Owner.Root = new ResultsViewModel(new List<BsonDocument>() { result }, Owner);
            }
            catch (BsonExtensions.BsonParseException ex)
            {
                LoggerHelper.Logger.Error("Exception while executing command", ex);
                Owner.SelectedViewIndex = 1;
                Owner.RawResult = ex.Message;
                Owner.Root = null;
                Messenger.Default.Send(new NotificationMessage<BsonExtensions.BsonParseException>(this, ex, Constants.CommandParseException));
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error("Exception while executing command", ex);
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
