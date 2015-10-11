using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MongoDB.Bson;
using System.Collections.ObjectModel;
using System.Windows;

namespace MongoDbGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ResultViewModel : ViewModelBase
    {
        protected string _result = string.Empty;

        public string Result
        {
            get
            {
                return _result;
            }
            set
            {
                Set(ref _result, value);
            }
        }

        protected string _id = string.Empty;

        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                Set(ref _id, value);
            }
        }

        protected int _index = 0;

        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                Set(ref _index, value);
            }
        }

        private ObservableCollection<ResultItemViewModel> _elements;
        public ObservableCollection<ResultItemViewModel> Elements
        {
            get { return _elements; }
            set
            {
                _elements = value;
                RaisePropertyChanged("Elements");
            }
        }

        public RelayCommand EditResult { get; set; }

        public RelayCommand ConfirmDeleteResult { get; set; }

        public RelayCommand CopyToClipboard { get; set; }

        /// <summary>
        /// Initializes a new instance of the ResultItemViewModel class.
        /// </summary>
        public ResultViewModel()
        {
            CopyToClipboard = new RelayCommand(() =>
            {
                Clipboard.SetText(Result);
            });
            _elements = new ObservableCollection<ResultItemViewModel>();
        }
    }
}