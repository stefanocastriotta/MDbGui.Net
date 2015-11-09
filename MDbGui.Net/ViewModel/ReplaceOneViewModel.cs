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
                ReplacementBsonDocument = Replacement.Deserialize<BsonDocument>(Constants.ReplacementProperty);
                Messenger.Default.Send(new NotificationMessage<ReplaceOneViewModel>(this, ((ResultsViewModel)Document.Parent).Owner, this, Constants.UpdateDocumentMessage));
            }
            catch (BsonExtensions.BsonParseException ex)
            {
                LoggerHelper.Logger.Error("Exception while updating a document", ex);
                ErrorMessage = ex.Message;
                Messenger.Default.Send(new NotificationMessage<BsonExtensions.BsonParseException>(this, ex, Constants.ReplaceOneParseException));
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                LoggerHelper.Logger.Error("Exception while updating a document", ex);
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            MessengerInstance.Unregister(this);
        }
    }
}
