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

        private ObservableCollection<BaseTreeviewViewModel> _children;
        public ObservableCollection<BaseTreeviewViewModel> Children
        {
            get { return _children; }
            set
            {
                _children = value;
                RaisePropertyChanged("Children");
            }
        }

        public FolderViewModel(string name, BaseTreeviewViewModel parent)
        {
            this._name = name;
            this._parent = parent;
            _children = new ObservableCollection<BaseTreeviewViewModel>();
        }

        public override void Cleanup()
        {
            base.Cleanup();
            foreach (var child in Children)
                child.Cleanup();
        }
    }
}
