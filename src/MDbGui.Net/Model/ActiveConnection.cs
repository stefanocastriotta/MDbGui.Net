﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDbGui.Net.Model
{
    public class ActiveConnection
    {
        public string Name { get; set; }

        public IMongoDbService Service { get; set; }
    }
}
