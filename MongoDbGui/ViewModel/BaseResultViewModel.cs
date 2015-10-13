using GalaSoft.MvvmLight;

namespace MongoDbGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class BaseResultViewModel : ViewModelBase
    {
        protected bool _isSelected;

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                Set(ref _isSelected, value);
            }
        }

        protected bool _isExpanded;

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                Set(ref _isExpanded, value);
            }
        }


        protected string _name;

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

        protected string _value;

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

        protected string _type;

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

        /// <summary>
        /// Initializes a new instance of the BaseResultViewModel class.
        /// </summary>
        public BaseResultViewModel()
        {
        }
    }
}