using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MDbGui.Net.Model
{
    public interface IMongoDbService
    {
        Task<MongoDbServer> ConnectAsync(ConnectionInfo connectionInfo);
        Task<List<BsonDocument>> ListDatabasesAsync();
        Task DropDatabaseAsync(string databaseName);
        Task<BsonValue> Eval(string databaseName, string function);
        Task<IMongoDatabase> CreateNewDatabaseAsync(string databaseName);
        Task<BsonDocument> ExecuteRawCommandAsync(string databaseName, string command, CancellationToken token);
        Task CreateCollectionAsync(string databaseName, string collection, CreateCollectionOptions options);
        Task RenameCollectionAsync(string databaseName, string oldName, string newName);
        Task DropCollectionAsync(string databaseName, string collection);
        Task<List<BsonDocument>> GetCollectionsAsync(string databaseName);
        Task<List<BsonDocument>> FindAsync(string databaseName, string collection, string filter, string sort, int? limit, int? skip, Guid operationComment, CancellationToken token);
        Task<long> CountAsync(string databaseName, string collection, string filter, CancellationToken token);
        Task<BulkWriteResult<BsonDocument>> InsertAsync(string databaseName, string collection, IEnumerable<BsonDocument> documents, CancellationToken token);
        Task<UpdateResult> UpdateAsync(string databaseName, string collection, string filter, BsonDocument document, bool multi, CancellationToken token);
        Task<ReplaceOneResult> ReplaceOneAsync(string databaseName, string collection, string filter, BsonDocument document, CancellationToken token);
        Task<DeleteResult> DeleteOneAsync(string databaseName, string collection, string filter, CancellationToken token);
    }
}
