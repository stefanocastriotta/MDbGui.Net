using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace MDbGui.Net.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CreateIndexViewModel : ViewModelBase
    {
        private MongoDbCollectionViewModel _collection;
        public MongoDbCollectionViewModel Collection
        {
            get { return _collection; }
            set
            {
                Set(ref _collection, value);
            }
        }

        protected string _name = string.Empty;

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

        protected bool _isNew = true;

        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                Set(ref _isNew, value);
            }
        }

        protected string _indexDefinition = string.Empty;
        public string IndexDefinition
        {
            get
            {
                return _indexDefinition;
            }
            set
            {
                Set(ref _indexDefinition, value);
            }
        }

        public RelayCommand CreateIndex { get; set; }

        /// <summary>
        /// Initializes a new instance of the CreateCollectionViewModel class.
        /// </summary>
        public CreateIndexViewModel()
        {
            CreateIndex = new RelayCommand(InnerCreateIndex, () =>
            {
                return !string.IsNullOrWhiteSpace(IndexDefinition);
            });
        }

        public void InnerCreateIndex()
        {
            if (IsNew)
                Messenger.Default.Send(new NotificationMessage<CreateIndexViewModel>(this, Collection, this, "CreateIndex"));
            else
                Messenger.Default.Send(new NotificationMessage<CreateIndexViewModel>(this, Collection, this, "RecreateIndex"));
        }

        public override void Cleanup()
        {
            base.Cleanup();
            MessengerInstance.Unregister(this);
        }
    }
}