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

        protected bool _isExpanded = false;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                Set(ref _isExpanded, value);
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


        //
        // Riepilogo:
        //     Gets or sets a value indicating whether to create the index in the background.
        private bool _background;
        public bool Background
        {
            get
            {
                return _background;
            }
            set
            {
                Set(ref _background, value);
            }
        }
        //
        // Riepilogo:
        //     Gets or sets the precision, in bits, used with geohash indexes.
        private int? _bits;
        public int? Bits
        {
            get
            {
                return _bits;
            }
            set
            {
                Set(ref _bits, value);
            }
        }
        //
        // Riepilogo:
        //     Gets or sets the size of a geohash bucket.
        private double? _bucketSize;
        public double? BucketSize
        {
            get
            {
                return _bucketSize;
            }
            set
            {
                Set(ref _bucketSize, value);
            }
        }
        //
        // Riepilogo:
        //     Gets or sets the default language.
        private string _defaultLanguage;
        public string DefaultLanguage
        {
            get
            {
                return _defaultLanguage;
            }
            set
            {
                Set(ref _defaultLanguage, value);
            }
        }
        //
        // Riepilogo:
        //     Gets or sets when documents expire (used with TTL indexes).
        private int? _expireAfter;
        public int? ExpireAfter
        {
            get
            {
                return _expireAfter;
            }
            set
            {
                Set(ref _expireAfter, value);
            }
        }
        //
        // Riepilogo:
        //     Gets or sets the language override.
        private string _languageOverride;
        public string LanguageOverride
        {
            get
            {
                return _languageOverride;
            }
            set
            {
                Set(ref _languageOverride, value);
            }
        }
        //
        // Riepilogo:
        //     Gets or sets the max value for 2d indexes.
        private double? _max;
        public double? Max
        {
            get
            {
                return _max;
            }
            set
            {
                Set(ref _max, value);
            }
        }
        //
        // Riepilogo:
        //     Gets or sets the min value for 2d indexes.
        private double? _min;
        public double? Min
        {
            get
            {
                return _min;
            }
            set
            {
                Set(ref _min, value);
            }
        }
        //
        // Riepilogo:
        //     Gets or sets a value indicating whether the index is a sparse index.
        private bool _sparse;
        public bool Sparse
        {
            get
            {
                return _sparse;
            }
            set
            {
                Set(ref _sparse, value);
            }
        }
        //
        // Riepilogo:
        //     Gets or sets the index version for 2dsphere indexes.
        private int? _sphereIndexVersion;
        public int? SphereIndexVersion
        {
            get
            {
                return _sphereIndexVersion;
            }
            set
            {
                Set(ref _sphereIndexVersion, value);
            }
        }
        //
        // Riepilogo:
        //     Gets or sets the storage engine options.
        private string _storageEngine;
        public string StorageEngine
        {
            get
            {
                return _storageEngine;
            }
            set
            {
                Set(ref _storageEngine, value);
            }
        }
        //
        // Riepilogo:
        //     Gets or sets the index version for text indexes.
        private int? _textIndexVersion;
        public int? TextIndexVersion
        {
            get
            {
                return _textIndexVersion;
            }
            set
            {
                Set(ref _textIndexVersion, value);
            }
        }
        //
        // Riepilogo:
        //     Gets or sets a value indicating whether the index is a unique index.
        private bool _unique;
        public bool Unique
        {
            get
            {
                return _unique;
            }
            set
            {
                Set(ref _unique, value);
            }
        }
        //
        // Riepilogo:
        //     Gets or sets the version of the index.
        private int? _version;
        public int? Version
        {
            get
            {
                return _version;
            }
            set
            {
                Set(ref _version, value);
            }
        }
        //
        // Riepilogo:
        //     Gets or sets the weights for text indexes.
        private string _weights;
        public string Weights
        {
            get
            {
                return _weights;
            }
            set
            {
                Set(ref _weights, value);
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