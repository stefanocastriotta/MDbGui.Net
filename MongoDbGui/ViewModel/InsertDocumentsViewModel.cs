using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MongoDbGui.Model;

namespace MongoDbGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class InsertDocumentsViewModel : ViewModelBase
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

        private string _documents = string.Empty;

        public string Documents
        {
            get
            {
                return _documents;
            }
            set
            {
                Set(ref _documents, value);
            }
        }

        private bool _inserting = false;

        public RelayCommand Insert { get; set; }

        /// <summary>
        /// Initializes a new instance of the InsertDocumentsViewModel class.
        /// </summary>
        public InsertDocumentsViewModel()
        {
            Documents = "[\n\t\n]";
            Insert = new RelayCommand(InnerInsert, () =>
            {
                return !_inserting;
            });
        }

        private void InnerInsert()
        {
            InsertDocumentsModel insertModel = new InsertDocumentsModel();
            insertModel.Collection = Collection;
            insertModel.Documents = Documents;
            _inserting = true;
            Messenger.Default.Send(new NotificationMessage<InsertDocumentsModel>(insertModel, "InsertDocuments"));
        }
    }
}