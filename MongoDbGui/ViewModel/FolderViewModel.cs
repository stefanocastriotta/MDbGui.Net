using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbGui.ViewModel
{
    public class FolderViewModel : BaseTreeviewViewModel
    {
        private BaseTreeviewViewModel _parent;

        public BaseTreeviewViewModel Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                Set(ref _parent, value);
            }
        }


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

        private ObservableCollection<BaseTreeviewViewModel> _children;
        public ObservableCollection<BaseTreeviewViewModel> Children
        {
            get { return _children; }
            set
            {
                _children = value;
                if (_children != null)
                    Count = _children.Count;
                RaisePropertyChanged("Children");
            }
        }

        private int? _count;

        public int? Count
        {
            get
            {
                return _count;
            }
            private set
            {
                Set(ref _count, value);
            }
        }

        public FolderViewModel(string name, BaseTreeviewViewModel parent)
        {
            this._name = name;
            this._parent = parent;
            _children = new ObservableCollection<BaseTreeviewViewModel>();
            _children.CollectionChanged += _children_CollectionChanged;
        }

        void _children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.IsExpanded && _children != null)
                Count = _children.Count;
        }
    }
}
