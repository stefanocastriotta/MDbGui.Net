using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using MongoDB.Bson;

namespace MDbGui.Net.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MongoDbUserViewModel : BaseTreeviewViewModel
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

        protected BsonDocument _userDocument;
        public BsonDocument UserDocument
        {
            get { return _userDocument; }
            set
            {
                Set(ref _userDocument, value);
            }
        }

        public RelayCommand EditUser { get; set; }

        public RelayCommand ConfirmDeleteUser { get; set; }

        /// <summary>
        /// Initializes a new instance of the MongoDbDatabaseViewModel class.
        /// </summary>
        public MongoDbUserViewModel(string name, BsonDocument userDocument)
        {
            _name = name;
            _userDocument = userDocument;
            EditUser = new RelayCommand(InternalEditUser);
            ConfirmDeleteUser = new RelayCommand(InternalConfirmDeleteUser);
        }

        private void InternalEditUser()
        {
            Messenger.Default.Send(new NotificationMessage<MongoDbUserViewModel>(this, ServiceLocator.Current.GetInstance<MainViewModel>(), this, "EditUser"));
        }

        private void InternalConfirmDeleteUser()
        {
            Messenger.Default.Send(new NotificationMessage<MongoDbUserViewModel>(this, ServiceLocator.Current.GetInstance<MainViewModel>(), this, "ConfirmDeleteUser"));
        }

        public override void Cleanup()
        {
            base.Cleanup();
            MessengerInstance.Unregister(this);
        }
    }
}