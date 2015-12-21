using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDbGui.Net.Model
{
    public class ConnectionInfo
    {
        public ConnectionInfo()
        {
        }

        public string Address { get; set; }

        public int Port { get; set; }

        public int Mode { get; set; }

        public string ConnectionString { get; set; }
    }
}
