using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;

namespace MDbGui.Net.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MongoDbIndexViewModel : BaseTreeviewViewModel
    {
        protected bool _iconVisible = true;

        public bool IconVisible
        {
            get { return _iconVisible; }
            set
            {
                Set(ref _iconVisible, value);
            }
        }

        private MongoDbCollectionViewModel _collection;
        public MongoDbCollectionViewModel Collection
        {
            get { return _collection; }
            set
            {
                Set(ref _collection, value);
            }
        }

        protected string _indexDefinition;

        public string IndexDefinition
        {
            get { return _indexDefinition; }
            set
            {
                Set(ref _indexDefinition, value);
            }
        }

        public RelayCommand EditIndex { get; set; }

        public RelayCommand ConfirmDropIndex { get; set; }

        /// <summary>
        /// Initializes a new instance of the MongoDbIndexViewModel class.
        /// </summary>
        public MongoDbIndexViewModel(MongoDbCollectionViewModel collection, string name)
        {
            _collection = collection;
            _name = name;
            ConfirmDropIndex = new RelayCommand(InternalConfirmDropIndex);
            EditIndex = new RelayCommand(InternalEditIndex);
        }

        private void InternalEditIndex()
        {
            Messenger.Default.Send(new NotificationMessage<MongoDbIndexViewModel>(this, ServiceLocator.Current.GetInstance<MainViewModel>(), this, "EditIndex"));
        }

        private void InternalConfirmDropIndex()
        {
            Messenger.Default.Send(new NotificationMessage<MongoDbIndexViewModel>(this, ServiceLocator.Current.GetInstance<MainViewModel>(), this, "ConfirmDropIndex"));
        }

        public override void Cleanup()
        {
            base.Cleanup();
            MessengerInstance.Unregister(this);
        }
    }
}