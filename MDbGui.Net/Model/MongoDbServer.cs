using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDbGui.Net.Model
{
    public class MongoDbServer
    {
        public MongoClient Client { get; set; }

        public List<BsonDocument> Databases { get; set; }
    }
}
