using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using MongoDbGui.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.IO;
using GalaSoft.MvvmLight.Messaging;

namespace MongoDbGui.ViewModel
{
    public class CollectionTabViewModel : BaseTabViewModel
    {
        public CollectionTabViewModel()
        {
            _sort = "{}";
            _find = "{}";
            _size = 50;
            ExecuteFind = new RelayCommand(InnerExecuteFind);
            ExecuteCount = new RelayCommand(InnerExecuteCount);
            ExecuteClose = new RelayCommand(InnerExecuteClose);
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

        private string _find = string.Empty;

        public string Find
        {
            get
            {
                return _find;
            }
            set
            {
                Set(ref _find, value);
            }
        }


        private string _sort = string.Empty;

        public string Sort
        {
            get
            {
                return _sort;
            }
            set
            {
                Set(ref _sort, value);
            }
        }

        private int _skip = 0;

        public int Skip
        {
            get
            {
                return _skip;
            }
            set
            {
                Set(ref _skip, value);
            }
        }

        private int _size;

        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                Set(ref _size, value);
            }
        }

        public RelayCommand ExecuteClose { get; set; }

        public void InnerExecuteClose()
        {
            Messenger.Default.Send(new NotificationMessage<CollectionTabViewModel>(this, "CloseTab"));
        }

        public RelayCommand ExecuteFind { get; set; }

        public async void InnerExecuteFind()
        {
            var results = await Collection.Database.Server.MongoDbService.Find(Collection.Database.Name, Collection.Name, Find, Sort, Size, Skip);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Results.Clear();
                foreach (var result in results)
                    Results.Add(result.ToJson(new JsonWriterSettings { Indent = true }));
            });
        }

        public RelayCommand ExecuteCount { get; set; }

        public async void InnerExecuteCount()
        {
            var result = await Collection.Database.Server.MongoDbService.Count(Collection.Database.Name, Collection.Name, Find);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Results.Clear();
                Results.Add(result.ToString());
            });
        }
    }
}
