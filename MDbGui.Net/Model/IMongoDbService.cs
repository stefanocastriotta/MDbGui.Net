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
        Task<BsonDocument> ExecuteRawCommandAsync(string databaseName, BsonDocument command, CancellationToken token);
        Task CreateCollectionAsync(string databaseName, string collection, CreateCollectionOptions options);
        Task RenameCollectionAsync(string databaseName, string oldName, string newName);
        Task DropCollectionAsync(string databaseName, string collection);
        Task<List<BsonDocument>> GetCollectionsAsync(string databaseName);
        Task<List<BsonDocument>> GetCollectionIndexesAsync(string databaseName, string collection);
        Task<string> CreateIndexAsync(string databaseName, string collection, BsonDocument indexDefinition, CreateIndexOptions options);
        Task DropIndexAsync(string databaseName, string collection, string indexName);
        Task<List<BsonDocument>> FindAsync(string databaseName, string collection, BsonDocument filter, BsonDocument sort, BsonDocument projection, int? limit, int? skip, bool explain, Guid operationComment, CancellationToken token);
        Task<long> CountAsync(string databaseName, string collection, BsonDocument filter, CancellationToken token);
        Task<List<BsonValue>> DistinctAsync(string databaseName, string collection, string field, BsonDocument filter, CancellationToken token);
        Task<BulkWriteResult<BsonDocument>> InsertAsync(string databaseName, string collection, BsonArray documents, CancellationToken token);
        Task<UpdateResult> UpdateAsync(string databaseName, string collection, BsonDocument filter, BsonDocument document, bool multi, CancellationToken token);
        Task<ReplaceOneResult> ReplaceOneAsync(string databaseName, string collection, BsonDocument filter, BsonDocument document, CancellationToken token);
        Task<DeleteResult> DeleteAsync(string databaseName, string collection, BsonDocument filter, bool justOne, CancellationToken token);
        Task<List<BsonDocument>> AggregateAsync(string databaseName, string collectionName, BsonArray pipeline, AggregateOptions options, bool explain, CancellationToken token);
        Task<List<BsonDocument>> MapReduceAsync(string databaseName, string collection, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<BsonDocument, BsonDocument> options, CancellationToken token);
    }
}
