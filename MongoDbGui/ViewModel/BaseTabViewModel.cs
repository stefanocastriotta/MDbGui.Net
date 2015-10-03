﻿using GalaSoft.MvvmLight;
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

        private ObservableCollection<string> _results;
        public ObservableCollection<string> Results
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
            _results = new ObservableCollection<string>();
        }
    }
}