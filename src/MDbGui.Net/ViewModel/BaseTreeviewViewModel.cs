﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MDbGui.Net.Utils;
using System.Collections.ObjectModel;

namespace MDbGui.Net.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class BaseTreeviewViewModel : ViewModelBase
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
                var oldValue = _isExpanded;
                Set(ref _isExpanded, value);
                Messenger.Default.Send(new PropertyChangedMessage<bool>(this, this, oldValue, _isExpanded, "IsExpanded"));
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

        protected bool _isEditing;

        public bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                Set(ref _isEditing, value);
            }
        }

        protected bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                Set(ref _isBusy, value);
            }
        }

        protected bool _isNew;

        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                Set(ref _isNew, value);
            }
        }

        protected double _sizeOnDisk;

        public double SizeOnDisk
        {
            get { return _sizeOnDisk; }
            set
            {
                Set(ref _sizeOnDisk, value);
                SizeOnDiskString = "Size: " + value.SizeSuffix();
            }
        }

        protected string _sizeOnDiskString;

        public string SizeOnDiskString
        {
            get { return _sizeOnDiskString; }
            set
            {
                Set(ref _sizeOnDiskString, value);
            }
        }

        protected long? _itemsCount;

        public long? ItemsCount
        {
            get { return _itemsCount; }
            set
            {
                Set(ref _itemsCount, value);
                ItemsCountString =value.HasValue ? "Items: " + value : "";
            }
        }

        protected string _itemsCountString;

        public string ItemsCountString
        {
            get { return _itemsCountString; }
            set
            {
                Set(ref _itemsCountString, value);
            }
        }

        protected ObservableCollection<BaseTreeviewViewModel> _children;
        public ObservableCollection<BaseTreeviewViewModel> Children
        {
            get { return _children; }
            set
            {
                Set(ref _children, value);
            }
        }

        public void UnselectAll()
        {
            if (Children != null)
            {
                foreach(var child in Children)
                {
                    child.IsSelected = false;
                    child.IsEditing = false;
                    child.UnselectAll();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the BaseTreeviewViewModel class.
        /// </summary>
        public BaseTreeviewViewModel()
        {
        }
    }
}