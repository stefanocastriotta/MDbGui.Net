using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using MDbGui.Net.Utils;

namespace MDbGui.Net.Model
{
    public class MongoDbService : IMongoDbService
    {
        private MongoClient client;

        #region Server Admin

        public async Task<MongoDbServer> ConnectAsync(ConnectionInfo connectionInfo)
        {
            if (connectionInfo.Mode == 1)
                client = new MongoClient(new MongoClientSettings() { Server = new MongoServerAddress(connectionInfo.Address, connectionInfo.Port), ConnectionMode = ConnectionMode.Direct });
            else
                client = new MongoClient(new MongoUrl(connectionInfo.ConnectionString));
            var databases = await client.ListDatabasesAsync();
            var serverStatus = await client.GetDatabase("admin").RunCommandAsync(new JsonCommand<BsonDocument>("{serverStatus:1}"));
            MongoDbServer server = new MongoDbServer();
            server.Client = client;
            server.Databases = await databases.ToListAsync();
            server.ServerStatus = serverStatus;
            return server;
        }

        public async Task<List<BsonDocument>> ListDatabasesAsync()
        {
            var databases = await client.ListDatabasesAsync();
            var databaseList = await databases.ToListAsync();
            return databaseList;
        }

        public async Task<IMongoDatabase> CreateNewDatabaseAsync(string databaseName)
        {
            var databases = await client.ListDatabasesAsync();
            var databasesList = await databases.ToListAsync();
            foreach (var database in databasesList)
            {
                if (database["name"] == databaseName)
                    throw new Exception("Database " + databaseName + " already existing");
            }
            return client.GetDatabase(databaseName);
            
        }

        public async Task DropDatabaseAsync(string databaseName)
        {
            await client.DropDatabaseAsync(databaseName);
        }

        public async Task<BsonDocument> ExecuteRawCommandAsync(string databaseName, string command, CancellationToken token)
        {
            var db = client.GetDatabase(databaseName);
            Guid operationComment = Guid.NewGuid();
            var result = await db.RunCommandAsync(new JsonCommand<BsonDocument>(command), null, token);
            return result;
        }

        public async Task<BsonValue> Eval(string databaseName, string function)
        {
            return await Task.Run(() =>
            {
                var server = client.GetServer();
                var database = server.GetDatabase(databaseName);
                var result = database.Eval(function);
                return result;
            });
        }

        #endregion

        #region Database Admin

        public async Task<List<BsonDocument>> GetCollectionsAsync(string databaseName)
        {
            var db = client.GetDatabase(databaseName);
            var collections = await db.ListCollectionsAsync();
            var listCollections = await collections.ToListAsync();
            return listCollections;
        }

        public async Task CreateCollectionAsync(string databaseName, string collection, CreateCollectionOptions options)
        {
            var db = client.GetDatabase(databaseName);
            await db.CreateCollectionAsync(collection, options);
        }

        public async Task RenameCollectionAsync(string databaseName, string oldName, string newName)
        {
            var db = client.GetDatabase(databaseName);
            await db.RenameCollectionAsync(oldName, newName);
        }

        public async Task DropCollectionAsync(string databaseName, string collection)
        {
            var db = client.GetDatabase(databaseName);
            await db.DropCollectionAsync(collection);
        }

        #endregion

        #region Collections admin

        public async Task<List<BsonDocument>> GetCollectionIndexesAsync(string databaseName, string collection)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            var cursor = await mongoCollection.Indexes.ListAsync();
            return await cursor.ToListAsync();
        }

        public async Task<string> CreateIndexAsync(string databaseName, string collection, string indexDefinition, CreateIndexOptions options)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            return await mongoCollection.Indexes.CreateOneAsync(indexDefinition.Deserialize<BsonDocument>(), options);
        }

        public async Task DropIndexAsync(string databaseName, string collection, string indexName)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            await mongoCollection.Indexes.DropOneAsync(indexName);
        }

        #endregion

        public async Task<List<BsonDocument>> FindAsync(string databaseName, string collection, string filter, string sort, string projection, int? limit, int? skip, bool explain, Guid operationComment, CancellationToken token)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);

            var find = mongoCollection.Find((string.IsNullOrWhiteSpace(filter) ? "{}" : filter).Deserialize<BsonDocument>(), new FindOptions() { Comment = operationComment.ToString(), Modifiers = explain ? BsonDocument.Parse("{ $explain: true }") : null });
            if (!string.IsNullOrWhiteSpace(sort))
                find = find.Sort(sort.Deserialize<BsonDocument>());
            if (!string.IsNullOrWhiteSpace(projection))
                find = find.Project(projection.Deserialize<BsonDocument>());

            find = find.Limit(limit).Skip(skip);
            
            return await find.ToListAsync();
        }

        public async Task<long> CountAsync(string databaseName, string collection, string filter, CancellationToken token)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            var result = await mongoCollection.CountAsync(filter.Deserialize<BsonDocument>(), null, token);
            return result;
        }

        public async Task<BulkWriteResult<BsonDocument>> InsertAsync(string databaseName, string collection, string documents, CancellationToken token)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            BsonArray array = documents.Deserialize<BsonArray>();
            var result = await mongoCollection.BulkWriteAsync(array.Select(d => new InsertOneModel<BsonDocument>(d.AsBsonDocument)), null, token);
            return result;
        }

        public async Task<UpdateResult> UpdateAsync(string databaseName, string collection, string filter, string document, bool multi, CancellationToken token)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            if (multi)
                return await mongoCollection.UpdateManyAsync(filter.Deserialize<BsonDocument>(), document.Deserialize<BsonDocument>(), null, token);
            else
                return await mongoCollection.UpdateOneAsync(filter.Deserialize<BsonDocument>(), document.Deserialize<BsonDocument>(), null, token);
        }

        public async Task<ReplaceOneResult> ReplaceOneAsync(string databaseName, string collection, string filter, string document, CancellationToken token)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            var result = await mongoCollection.ReplaceOneAsync(filter.Deserialize<BsonDocument>(), document.Deserialize<BsonDocument>(), null, token);
            return result;
        }

        public async Task<DeleteResult> DeleteAsync(string databaseName, string collection, string filter, bool justOne, CancellationToken token)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            if (justOne)
                return await mongoCollection.DeleteOneAsync(filter.Deserialize<BsonDocument>(), token);
            else
                return await mongoCollection.DeleteManyAsync(filter.Deserialize<BsonDocument>(), token);
        }

        public async Task<List<BsonDocument>> AggregateAsync(string databaseName, string collectionName, string pipeline, AggregateOptions options, bool explain, CancellationToken token)
        {
            return await Task.Run(() =>
            {
                var server = client.GetServer();
                var database = server.GetDatabase(databaseName);
                var collection = database.GetCollection(collectionName);
                List<BsonDocument> result;
                if (!explain)
                    result = collection.Aggregate(new AggregateArgs()
                    {
                        AllowDiskUse = options.AllowDiskUse,
                        BatchSize = options.BatchSize,
                        MaxTime = options.MaxTime,
                        Pipeline = pipeline.Deserialize<BsonArray>().Select(s => s.AsBsonDocument)
                    }).ToList();
                else
                {
                    var explainResult = collection.AggregateExplain(new AggregateArgs()
                    {
                        AllowDiskUse = options.AllowDiskUse,
                        BatchSize = options.BatchSize,
                        MaxTime = options.MaxTime,
                        Pipeline = pipeline.Deserialize<BsonArray>().Select(s => s.AsBsonDocument)
                    });

                    result = new List<BsonDocument>();
                    result.Add(explainResult.Response);
                }
                return result;
            }, token);
        }
    }
}