using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace MongoDbGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ResultItemViewModel : ViewModelBase
    {
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

        private string _value = string.Empty;

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                Set(ref _value, value);
            }
        }

        private string _type = string.Empty;

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                Set(ref _type, value);
            }
        }

        private ObservableCollection<ResultItemViewModel> _children;
        public ObservableCollection<ResultItemViewModel> Children
        {
            get { return _children; }
            set
            {
                _children = value;
                RaisePropertyChanged("Elements");
            }
        }

        /// <summary>
        /// Initializes a new instance of the ResultItemViewModel class.
        /// </summary>
        public ResultItemViewModel()
        {
            _children = new ObservableCollection<ResultItemViewModel>();
        }
    }
}