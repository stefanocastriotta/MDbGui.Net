using GalaSoft.MvvmLight.CommandWpf;
using MDbGui.Net.Utils;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDbGui.Net.ViewModel.Operations
{
    public class MongoDbEvalOperationViewModel : MongoDbOperationViewModel
    {
        public MongoDbEvalOperationViewModel(TabViewModel owner) : base(owner)
        {
            Name = "Eval";
            DisplayName = "Eval";
            ExecuteEval = new RelayCommand(InnerExecuteEval);
        }

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
            Owner.Executing = true;
            try
            {
                var result = await Owner.Service.Eval(Owner.Database, EvalFunction);

                Owner.RawResult = result.ToJson(Options.JsonWriterSettings);

                if (result.IsBsonDocument)
                    Owner.Root = new ResultsViewModel(new List<BsonDocument>() { result.AsBsonDocument }, Owner);
                else
                    Owner.Root = new ResultsViewModel(new List<BsonDocument>() { new BsonDocument().Add("result", result) }, Owner);
            }
            catch (Exception ex)
            {
                LoggerHelper.Logger.Error("Exception while executing Eval command", ex);
                Owner.RawResult = ex.Message;
                Owner.Root = null;
            }
            finally
            {
                Owner.SelectedViewIndex = 1;
                Owner.Executing = false;
                Owner.ShowPager = false;
            }
        }
    }
}
