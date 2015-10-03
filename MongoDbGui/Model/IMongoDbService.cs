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
        Task<IMongoDatabase> CreateNewDatabase(string databaseName);
        Task<BsonDocument> ExecuteRawCommand(string databaseName, string command);
        Task CreateCollection(string databaseName, string collection);
        Task RenameCollection(string databaseName, string oldName, string newName);
        Task<List<BsonDocument>> GetCollections(string databaseName);
        Task<List<BsonDocument>> Find(string databaseName, string collection, string filter, string sort, int? limit, int? skip);
        Task<long> Count(string databaseName, string collection, string filter);
    }
}
