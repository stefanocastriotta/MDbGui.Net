using MongoDB.Bson;
using MongoDB.Driver;
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
        Task<List<BsonDocument>> GetCollections(MongoClient client, string databaseName);
        Task<BsonDocument> ExecuteRawCommand(MongoClient client, string databaseName, string command);
    }
}
