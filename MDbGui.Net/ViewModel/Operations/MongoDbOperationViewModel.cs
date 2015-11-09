using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDbGui.Net.ViewModel.Operations
{
    public abstract class MongoDbOperationViewModel : ViewModelBase
    {
        public MongoDbOperationViewModel(TabViewModel owner)
        {
            Owner = owner;
        }

        protected TabViewModel Owner { get; set; }

        private string _name = string.Empty;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                Set(ref _name, value);
            }
        }

        private string _displayName = string.Empty;

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                Set(ref _displayName, value);
            }
        }
    }
}
