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
    public class ResultItemViewModel : BaseResultViewModel
    {
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