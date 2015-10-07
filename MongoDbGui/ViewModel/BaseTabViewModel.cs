using GalaSoft.MvvmLight;
using MongoDB.Bson;
using MongoDbGui.Model;
using System.Collections.ObjectModel;

namespace MongoDbGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class BaseTabViewModel : ViewModelBase
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

        private string _rawResult = string.Empty;

        public string RawResult
        {
            get
            {
                return _rawResult;
            }
            set
            {
                Set(ref _rawResult, value);
            }
        }


        private ObservableCollection<ResultItemViewModel> _results;
        public ObservableCollection<ResultItemViewModel> Results
        {
            get { return _results; }
            set
            {
                _results = value;
                RaisePropertyChanged("Results");
            }
        }

        /// <summary>
        /// Initializes a new instance of the TabViewModel class.
        /// </summary>
        public BaseTabViewModel()
        {
            _results = new ObservableCollection<ResultItemViewModel>();
        }
    }
}