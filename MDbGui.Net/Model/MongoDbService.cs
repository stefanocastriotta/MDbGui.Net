using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

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
            MongoDbServer server = new MongoDbServer();
            server.Client = client;
            server.Databases = await databases.ToListAsync();

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

        public async Task<List<BsonDocument>> FindAsync(string databaseName, string collection, string filter, string sort, string projection, int? limit, int? skip, bool explain, Guid operationComment, CancellationToken token)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            var find = mongoCollection.Find(BsonDocument.Parse(string.IsNullOrWhiteSpace(filter) ? "{}" : filter), new FindOptions() { Comment = operationComment.ToString(), Modifiers = explain ? BsonDocument.Parse("{ $explain: true }") : null });
            if (!string.IsNullOrWhiteSpace(sort))
                find = find.Sort(BsonDocument.Parse(sort));
            if (!string.IsNullOrWhiteSpace(projection))
                find = find.Project(BsonDocument.Parse(projection));

            find = find.Limit(limit).Skip(skip);
            
            return await find.ToListAsync();
        }

        public async Task<long> CountAsync(string databaseName, string collection, string filter, CancellationToken token)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            var result = await mongoCollection.CountAsync(BsonDocument.Parse(filter), null, token);
            return result;
        }

        public async Task<BulkWriteResult<BsonDocument>> InsertAsync(string databaseName, string collection, IEnumerable<BsonDocument> documents, CancellationToken token)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            var result = await mongoCollection.BulkWriteAsync(documents.Select(d => new InsertOneModel<BsonDocument>(d)), null, token);
            return result;
        }

        public async Task<UpdateResult> UpdateAsync(string databaseName, string collection, string filter, BsonDocument document, bool multi, CancellationToken token)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            if (multi)
                return await mongoCollection.UpdateManyAsync(BsonDocument.Parse(filter), document, null, token);
            else
                return await mongoCollection.UpdateOneAsync(BsonDocument.Parse(filter), document, null, token);
        }

        public async Task<ReplaceOneResult> ReplaceOneAsync(string databaseName, string collection, string filter, BsonDocument document, CancellationToken token)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            var result = await mongoCollection.ReplaceOneAsync(BsonDocument.Parse(filter), document, null, token);
            return result;
        }

        public async Task<DeleteResult> DeleteAsync(string databaseName, string collection, string filter, bool justOne, CancellationToken token)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            if (justOne)
                return await mongoCollection.DeleteOneAsync(BsonDocument.Parse(filter), token);
            else
                return await mongoCollection.DeleteManyAsync(BsonDocument.Parse(filter), token);
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
                        Pipeline = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(pipeline).Select(s => s.AsBsonDocument)
                    }).ToList();
                else
                {
                    var explainResult = collection.AggregateExplain(new AggregateArgs()
                    {
                        AllowDiskUse = options.AllowDiskUse,
                        BatchSize = options.BatchSize,
                        MaxTime = options.MaxTime,
                        Pipeline = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(pipeline).Select(s => s.AsBsonDocument)
                    });

                    result = new List<BsonDocument>();
                    result.Add(explainResult.Response);
                }
                return result;
            }, token);
        }
    }
}