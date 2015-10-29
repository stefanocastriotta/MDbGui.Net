using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDbGui.Net.Model
{
    public class DatabaseCommand
    {
        public string Command { get; set; }

        public bool ExecuteImmediately { get; set; }
    }
}
