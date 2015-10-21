using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDbGui.Model
{
    public interface IMongoDbService
    {
        Task<MongoDbServer> ConnectAsync(ConnectionInfo connectionInfo);
        Task<List<BsonDocument>> ListDatabasesAsync();
        Task<IMongoDatabase> CreateNewDatabaseAsync(string databaseName);
        Task<BsonDocument> ExecuteRawCommandAsync(string databaseName, string command);
        Task CreateCollectionAsync(string databaseName, string collection, CreateCollectionOptions options);
        Task RenameCollectionAsync(string databaseName, string oldName, string newName);
        Task DropCollectionAsync(string databaseName, string collection);
        Task<List<BsonDocument>> GetCollectionsAsync(string databaseName);
        Task<List<BsonDocument>> FindAsync(string databaseName, string collection, string filter, string sort, int? limit, int? skip, CancellationToken token);
        Task<long> CountAsync(string databaseName, string collection, string filter);
        Task<BulkWriteResult<BsonDocument>> InsertAsync(string databaseName, string collection, IEnumerable<BsonDocument> documents);
        Task<ReplaceOneResult> ReplaceOneAsync(string databaseName, string collection, string filter, BsonDocument document);
    }
}
