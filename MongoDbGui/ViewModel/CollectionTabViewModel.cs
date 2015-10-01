﻿using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using MongoDbGui.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.IO;

namespace MongoDbGui.ViewModel
{
    public class CollectionTabViewModel : BaseTabViewModel
    {
        public CollectionTabViewModel(IMongoDbService mongoDbService) : base(mongoDbService)
        {
            _sort = "{}";
            _find = "{}";
            _size = 50;
            ExecuteFind = new RelayCommand(InnerExecuteFind);
            ExecuteCount = new RelayCommand(InnerExecuteCount);
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

        public RelayCommand ExecuteFind { get; set; }

        public async void InnerExecuteFind()
        {
            var results = await _mongoDbService.Find(Collection.Database.Server.Client, Collection.Database.Name, Collection.Name, Find, Sort, Size, Skip);
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
            var result = await _mongoDbService.Count(Collection.Database.Server.Client, Collection.Database.Name, Collection.Name, Find);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Results.Clear();
                Results.Add(result.ToString());
            });
        }
    }
}
