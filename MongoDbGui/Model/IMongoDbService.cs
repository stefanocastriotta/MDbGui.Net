using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbGui.Model
{
    public interface IMongoDbService
    {
        Task<MongoDbServer> Connect(ConnectionInfo connectionInfo);
        Task<List<BsonDocument>> GetCollections(MongoDbServer server, string databaseName);
    }
}
