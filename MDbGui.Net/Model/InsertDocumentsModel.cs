using MDbGui.Net.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDbGui.Net.Model
{
    public class InsertDocumentsModel
    {
        public MongoDbCollectionViewModel Collection { get; set; }

        public string Documents { get; set; }
    }
}
