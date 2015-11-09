using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MongoDB.Driver;
using MDbGui.Net.Model;
using MDbGui.Net.Utils;

namespace MDbGui.Net.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        private string _address = string.Empty;
        
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                Set(ref _address, value);
            }
        }

        private int _port = 27017;

        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                Set(ref _port, value);
            }
        }

        private string _connectionString = string.Empty;

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                Set(ref _connectionString, value);
            }
        }


        private bool _hostPortMode = true;

        public bool HostPortMode
        {
            get
            {
                return _hostPortMode;
            }
            set
            {
                Set(ref _hostPortMode, value);
            }
        }

        private bool _connectionStringMode = false;

        public bool ConnectionStringMode
        {
            get
            {
                return _connectionStringMode;
            }
            set
            {
                Set(ref _connectionStringMode, value);
            }
        }

        private bool _connecting = false;

        public RelayCommand Connect { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public LoginViewModel()
        {
            Connect = new RelayCommand(ConnectToDatabase, () =>
            {
                if (HostPortMode)
                    return !_connecting && !string.IsNullOrWhiteSpace(Address) && Port > 0;
                else
                    return !_connecting && !string.IsNullOrWhiteSpace(ConnectionString);
            });
        }

        public void ConnectToDatabase()
        {
            _connecting = true;
            MongoClient client;
            ConnectionInfo info = new ConnectionInfo() { Address = Address, Port = Port, Mode = HostPortMode ? 1 : 2, ConnectionString = ConnectionString };
            if (HostPortMode)
                client = new MongoClient(new MongoClientSettings() { Server = new MongoServerAddress(Address, Port) });
            else
            {
                client = new MongoClient(new MongoUrl(ConnectionString));
                info.Address = client.Settings.Server.Host;
                info.Port = client.Settings.Server.Port;
            }

            Messenger.Default.Send(new NotificationMessage<ConnectionInfo>(info, Constants.LoggingInMessage));
        }

        public override void Cleanup()
        {
 	        base.Cleanup();
            MessengerInstance.Unregister(this);
        }
    }
}