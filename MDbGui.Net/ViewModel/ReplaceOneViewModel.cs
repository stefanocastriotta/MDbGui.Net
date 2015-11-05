using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MDbGui.Net.Utils;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDbGui.Net.ViewModel
{
    public class ReplaceOneViewModel : ViewModelBase
    {
        public ReplaceOneViewModel()
        {
            Replace = new RelayCommand(InnerExecuteReplace, () =>
            {
                return !string.IsNullOrWhiteSpace(Replacement);
            });
        }

        public DocumentResultViewModel Document { get; set; }

        private string _replacement;
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

        private string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                Set(ref _errorMessage, value);
            }
        }

        public BsonDocument ReplacementBsonDocument { get; set; }

        public RelayCommand Replace { get; set; }

        private void InnerExecuteReplace()
        {
            try
            {
                ReplacementBsonDocument = Replacement.Deserialize<BsonDocument>("Replacement");
                Messenger.Default.Send(new NotificationMessage<ReplaceOneViewModel>(this, ((ResultsViewModel)Document.Parent).Owner, this, "UpdateDocument"));
            }
            catch (BsonExtensions.BsonParseException ex)
            {
                Utils.LoggerHelper.Logger.Error("Exception while updating a document", ex);
                ErrorMessage = ex.Message;
                Messenger.Default.Send(new NotificationMessage<BsonExtensions.BsonParseException>(this, ex, "ReplaceOneParseException"));
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                Utils.LoggerHelper.Logger.Error("Exception while updating a document", ex);
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            MessengerInstance.Unregister(this);
        }
    }
}
