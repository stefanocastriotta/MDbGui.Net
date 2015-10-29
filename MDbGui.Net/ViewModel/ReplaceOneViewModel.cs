using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
            Replace = new RelayCommand(InnerExecuteReplace);
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

        public RelayCommand Replace { get; set; }

        private void InnerExecuteReplace()
        {
            Messenger.Default.Send(new NotificationMessage<ReplaceOneViewModel>(this, ((ResultsViewModel)Document.Parent).Owner, this, "UpdateDocument"));
        }

        public override void Cleanup()
        {
            base.Cleanup();
            MessengerInstance.Unregister(this);
        }
    }
}
