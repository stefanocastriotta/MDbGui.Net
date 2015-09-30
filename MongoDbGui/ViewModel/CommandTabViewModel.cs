using GalaSoft.MvvmLight.CommandWpf;
using MongoDbGui.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbGui.ViewModel
{
    public class CommandTabViewModel : BaseTabViewModel
    {
        private MongoDbDatabaseViewModel _database;
        public MongoDbDatabaseViewModel Database
        {
            get { return _database; }
            set
            {
                Set(ref _database, value);
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

        public CommandTabViewModel(IMongoDbService mongoDbService) : base(mongoDbService)
        {
            ExecuteCommand = new RelayCommand(InnerExecuteCommand, () =>
            {
                return !string.IsNullOrWhiteSpace(Command);
            });
        }

        public RelayCommand ExecuteCommand { get; set; }

        public async void InnerExecuteCommand()
        {
            
        }

    }
}
