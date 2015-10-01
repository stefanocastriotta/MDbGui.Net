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
        Task<IMongoDatabase> CreateNewDatabase(MongoClient client, string databaseName, string collection);
        Task<BsonDocument> ExecuteRawCommand(MongoClient client, string databaseName, string command);
        void CreateCollection(MongoClient client, string databaseName, string collection);
        Task<List<BsonDocument>> GetCollections(MongoClient client, string databaseName);
        Task<List<BsonDocument>> Find(MongoClient client, string databaseName, string collection, string filter, string sort, int? limit, int? skip);
        Task<long> Count(MongoClient client, string databaseName, string collection, string filter);
    }
}
