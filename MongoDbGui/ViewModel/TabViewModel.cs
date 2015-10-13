using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using MongoDbGui.Model;
using MongoDbGui.Utils;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace MongoDbGui.ViewModel
{
    public class TabViewModel : ViewModelBase
    {
        public TabViewModel()
        {
            _results = new ObservableCollection<ResultViewModel>();
            ExecutingTimer.Tick += ExecutingTimer_Tick;
            ExecutingTimer.Interval = TimeSpan.FromMilliseconds(100);

            _sort = "{}";
            _find = "{}";
            _size = 50;

            _command = "{}";
            
            ExecuteFind = new RelayCommand(() =>
            {
                Skip = 0;
                InnerExecuteFind();
            });
            DoPaging = new RelayCommand(InnerExecuteFind);
            ExecuteCount = new RelayCommand(InnerExecuteCount);
            ExecuteClose = new RelayCommand(InnerExecuteClose);
            PageBack = new RelayCommand(InnerPageBack);
            PageForward = new RelayCommand(InnerPageForward);
            CopyToClipboard = new RelayCommand(() =>
            {
                Clipboard.SetText(RawResult);
            });

            ExecuteCommand = new RelayCommand(InnerExecuteCommand);
        }

        private string _commandType = string.Empty;

        public string CommandType
        {
            get
            {
                return _commandType;
            }
            set
            {
                Set(ref _commandType, value);
            }
        }


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

        private bool _executing;

        public bool Executing
        {
            get
            {
                return _executing;
            }
            set
            {
                if (value && !_executing)
                {
                    ExecutingTime = TimeSpan.Zero.ToString("hh':'mm':'ss'.'fff");
                    _executingStartTime = DateTime.Now;
                    ExecutingTimer.Start();
                }
                else if (!value && _executing)
                {
                    ExecutingTimer.Stop();
                }
                Set(ref _executing, value);
            }
        }

        private DispatcherTimer ExecutingTimer = new DispatcherTimer();

        private DateTime _executingStartTime;

        private string _executingTime = string.Empty;

        public string ExecutingTime
        {
            get
            {
                return _executingTime;
            }
            set
            {
                Set(ref _executingTime, value);
            }
        }


        private string _rawResult = string.Empty;

        public string RawResult
        {
            get
            {
                return _rawResult;
            }
            set
            {
                Set(ref _rawResult, value);
            }
        }

        private int _selectedViewIndex = 0;

        public int SelectedViewIndex
        {
            get
            {
                return _selectedViewIndex;
            }
            set
            {
                Set(ref _selectedViewIndex, value);
            }
        }


        private ObservableCollection<ResultViewModel> _results;
        public ObservableCollection<ResultViewModel> Results
        {
            get { return _results; }
            set
            {
                _results = value;
                RaisePropertyChanged("Results");
            }
        }

        void ExecutingTimer_Tick(object sender, System.EventArgs e)
        {
            ExecutingTime = (DateTime.Now - _executingStartTime).ToString("hh':'mm':'ss'.'fff");
        }


        private MongoDbServerViewModel _server;
        public MongoDbServerViewModel Server
        {
            get { return _server; }
            set
            {
                Set(ref _server, value);
            }
        }

        private MongoDbDatabaseViewModel _database;
        public MongoDbDatabaseViewModel Database
        {
            get { return _database; }
            set
            {
                Set(ref _database, value);
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


        private string _command = string.Empty;

        public string Command
        {
            get
            {
                return _command;
            }
            set
            {
                Set(ref _command, value);
            }
        }


        public RelayCommand ExecuteClose { get; set; }

        public void InnerExecuteClose()
        {
            Messenger.Default.Send(new NotificationMessage<TabViewModel>(this, "CloseTab"));
        }

        public RelayCommand ExecuteFind { get; set; }

        public async void InnerExecuteFind()
        {
            Executing = true;
            var results = await Collection.Database.Server.MongoDbService.FindAsync(Collection.Database.Name, Collection.Name, Find, Sort, Size, Skip);
            Executing = false;
            StringBuilder sb = new StringBuilder();
            int index = 1;
            sb.Append("[");
            foreach (var result in results)
            {
                sb.AppendLine();
                sb.Append("/* # ");
                sb.Append(index.ToString());
                sb.AppendLine(" */");
                sb.AppendLine(result.ToJson(new JsonWriterSettings { Indent = true }));
                sb.Append(",");
                index++;
            }
            if (results.Count > 0)
                sb.Length -= 1;
            sb.AppendLine();
            sb.Append("]");

            RawResult = sb.ToString();

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Results.Clear();
                foreach (var result in results)
                    Results.Add(new ResultViewModel() { 
                        Result = result.ToJson(new JsonWriterSettings { Indent = true }),
                        Name = "#" + (results.IndexOf(result) + 1) + " (" + result["_id"].ToString() + ")",
                        Id = result["_id"].ToString(),
                        Value = "(" + result.ElementCount + ") fields",
                        Type = result.BsonType.ToString(),
                        Elements = new ObservableCollection<ResultItemViewModel>(result.ToResultItemViewModel())
                    });
            });
            
        }

        public RelayCommand ExecuteCount { get; set; }

        public async void InnerExecuteCount()
        {
            Executing = true;
            var result = await Collection.Database.Server.MongoDbService.CountAsync(Collection.Database.Name, Collection.Name, Find);
            Executing = false;

            RawResult = result.ToString();
            SelectedViewIndex = 0;
            
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Results.Clear();
            });
        }

        public RelayCommand PageBack { get; set; }

        public void InnerPageBack()
        {
            Skip -= Size;
            Skip = Math.Max(0, Skip);
            InnerExecuteFind();
        }

        public RelayCommand PageForward { get; set; }

        public void InnerPageForward()
        {
            Skip += Size;
            InnerExecuteFind();
        }

        public RelayCommand DoPaging { get; set; }

        public RelayCommand CopyToClipboard { get; set; }


        public RelayCommand ExecuteCommand { get; set; }

        public async void InnerExecuteCommand()
        {
            Executing = true;
            try
            {
                var result = await Database.Server.MongoDbService.ExecuteRawCommandAsync(Database.Name, Command);

                RawResult = result.ToJson(new JsonWriterSettings { Indent = true });

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Results.Clear();
                    Results.Add(new ResultViewModel()
                    {
                        Result = result.ToJson(new JsonWriterSettings { Indent = true }),
                        Name = "#1",
                        Value = "(" + result.ElementCount + ") fields",
                        Type = result.BsonType.ToString(),
                    });
                });
            }
            catch (Exception ex)
            {
                RawResult = ex.Message;
                SelectedViewIndex = 0;
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Results.Clear();
                });
            }
            finally
            {
                Executing = false;
            }
        }

    }
}
