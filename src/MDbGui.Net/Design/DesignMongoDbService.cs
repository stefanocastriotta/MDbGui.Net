using System;
using MDbGui.Net.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading;

namespace MDbGui.Net.Design
{
    public class DesignMongoDbService : IMongoDbService
    {
        public Task<MongoDbServer> ConnectAsync(ConnectionInfo connectionInfo)
        {
            // Use this to create design time data

            MongoDbServer server = new MongoDbServer();
            server.Databases = new List<BsonDocument>();
            server.Databases.Add(new BsonDocument { { "name", "TestDatabase1" } });
            return Task.FromResult(server);
        }

        public Task<List<BsonDocument>> ListDatabasesAsync()
        {
            return Task.FromResult(new List<BsonDocument>());
        }

        public Task DropDatabaseAsync(string databaseName)
        {
            return new Task(() => { });
        }

        public Task<BsonValue> Eval(string databaseName, string function)
        {
            return Task.FromResult<BsonValue>(new BsonDocument());
        }

        public Task<IMongoDatabase> CreateNewDatabaseAsync(string databaseName)
        {
            return Task.FromResult<IMongoDatabase>(null);
        }

        public Task CreateCollectionAsync(string databaseName, string collection, CreateCollectionOptions options)
        {
            return new Task(() => { });
        }

        public Task RenameCollectionAsync(string databaseName, string oldName, string newName)
        {
            return new Task(() => { });
        }

        public Task DropCollectionAsync(string databaseName, string collection)
        {
            return new Task(() => { });
        }

        public Task<List<BsonDocument>> GetCollectionsAsync(string databaseName)
        {
            // Use this to create design time data

            return Task.FromResult(new List<BsonDocument>());
        }

        public Task<List<BsonDocument>> GetCollectionIndexesAsync(string databaseName, string collection)
        {
            return Task.FromResult(new List<BsonDocument>());
        }

        public Task<string> CreateIndexAsync(string databaseName, string collection, BsonDocument indexDefinition, CreateIndexOptions options)
        {
            return Task.FromResult("");
        }

        public Task DropIndexAsync(string databaseName, string collection, string indexName)
        {
            return new Task(() => { });
        }

        public Task<BsonDocument> ExecuteRawCommandAsync(string databaseName, BsonDocument command, CancellationToken token)
        {
            // Use this to create design time data

            return Task.FromResult(new BsonDocument());
        }

        public Task<List<BsonDocument>> FindAsync(string databaseName, string collection, BsonDocument filter, BsonDocument sort, BsonDocument projection, int? limit, int? skip, bool explain, Guid operationComment, CancellationToken token)
        {
            // Use this to create design time data

            return Task.FromResult(new List<BsonDocument>());
        }

        public Task<long> CountAsync(string databaseName, string collection, BsonDocument filter, CancellationToken token)
        {
            // Use this to create design time data

            return Task.FromResult<long>(100);
        }

        public Task<List<BsonValue>> DistinctAsync(string databaseName, string collection, string field, BsonDocument filter, CancellationToken token)
        {
            return Task.FromResult<List<BsonValue>>(new List<BsonValue>());
        }

        public Task<BulkWriteResult<BsonDocument>> InsertAsync(string databaseName, string collection, BsonArray documents, CancellationToken token)
        {
            return null;
        }

        public Task<ReplaceOneResult> ReplaceOneAsync(string databaseName, string collection, BsonDocument filter, BsonDocument document, CancellationToken token)
        {
            return null;
        }

        public Task<UpdateResult> UpdateAsync(string databaseName, string collection, BsonDocument filter, BsonDocument document, bool multi, CancellationToken token)
        {
            return null;
        }

        public Task<DeleteResult> DeleteAsync(string databaseName, string collection, BsonDocument filter, bool justOne, CancellationToken token)
        {
            return null;
        }

        public Task<List<BsonDocument>> AggregateAsync(string databaseName, string collectionName, BsonArray pipeline, AggregateOptions options, bool explain, CancellationToken token)
        {
            return null;
        }

        public Task<List<BsonDocument>> MapReduceAsync(string databaseName, string collection, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<BsonDocument, BsonDocument> options, CancellationToken token)
        {
            return Task.FromResult(new List<BsonDocument>());
        }
    }
}