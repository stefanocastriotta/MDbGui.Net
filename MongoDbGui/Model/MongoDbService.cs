using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using MongoDbGui.Utils;

namespace MongoDbGui.Model
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

        public async Task<BsonDocument> ExecuteRawCommandAsync(string databaseName, string command)
        {
            var db = client.GetDatabase(databaseName);
            var result = await db.RunCommandAsync(new JsonCommand<BsonDocument>(command));
            return result;
        }

        public async Task<BsonDocument> GetCurrentOp()
        {
            return await Task.Run(() =>
            {
                var server = client.GetServer();
                var database = server.GetDatabase("local");
                var result = database.GetCurrentOp();
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

        public async Task<List<BsonDocument>> FindAsync(string databaseName, string collection, string filter, string sort, int? limit, int? skip, CancellationToken token)
        {

            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            Guid operationComment = Guid.NewGuid();
            var task = mongoCollection.Find(BsonDocument.Parse(filter), new FindOptions() { Comment = operationComment.ToString() }).Sort(BsonDocument.Parse(sort)).Limit(limit).Skip(skip).ToListAsync(token);
            try
            {
                await task.WithCancellation(token);
                return task.Result;
            }
            catch (OperationCanceledException)
            {
                if (!task.IsCompleted)
                {
                    task.ContinueWith(t =>
                    {
                        if (t.Exception != null)
                        {

                        }
                    });
                    Task.Run(async () =>
                    {
                        var server = client.GetServer();
                        var database = server.GetDatabase(databaseName);
                        var currentOp = database.Eval("function() { return db.currentOP(); }");
                        if (currentOp != null)
                        {
                            var operation = currentOp.AsBsonDocument["inprog"].AsBsonArray.FirstOrDefault(item => item.AsBsonDocument.Contains("query") && item.AsBsonDocument["query"].AsBsonDocument.Contains("$comment") && item.AsBsonDocument["query"]["$comment"].AsString == operationComment.ToString());
                            if (operation != null)
                            {
                                database.Eval(new BsonJavaScript(string.Format("function() {{ return db.killOp({0}); }}", operation["opid"].AsInt32)));
                            }
                        }
                    });
                }
                return null;
            }
        }

        public async Task<long> CountAsync(string databaseName, string collection, string filter)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            var result = await mongoCollection.CountAsync(BsonDocument.Parse(filter));
            return result;
        }

        public async Task<BulkWriteResult<BsonDocument>> InsertAsync(string databaseName, string collection, IEnumerable<BsonDocument> documents)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            var result = await mongoCollection.BulkWriteAsync(documents.Select(d => new InsertOneModel<BsonDocument>(d)));
            return result;
        }

        public async Task<UpdateResult> UpdateAsync(string databaseName, string collection, string filter, BsonDocument document)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            var result = await mongoCollection.UpdateManyAsync(BsonDocument.Parse(filter), document);
            return result;
        }

        public async Task<ReplaceOneResult> ReplaceOneAsync(string databaseName, string collection, string filter, BsonDocument document)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            var result = await mongoCollection.ReplaceOneAsync(BsonDocument.Parse(filter), document);
            return result;
        }
    }
}