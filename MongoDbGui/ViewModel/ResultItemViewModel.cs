using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows;

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

        public RelayCommand EditResult { get; set; }

        public RelayCommand ConfirmDeleteResult { get; set; }

        public RelayCommand CopyToClipboard { get; set; }

        /// <summary>
        /// Initializes a new instance of the ResultItemViewModel class.
        /// </summary>
        public ResultItemViewModel()
        {
            CopyToClipboard = new RelayCommand(() =>
            {
                Clipboard.SetText(Result);
            });
        }
    }
}