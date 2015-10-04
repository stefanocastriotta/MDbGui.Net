using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MongoDbGui.Model
{
    public class MongoDbService : IMongoDbService
    {
        private MongoClient client;

        #region Server Admin

        public async Task<MongoDbServer> Connect(ConnectionInfo connectionInfo)
        {
            if (connectionInfo.Mode == 1)
                client = new MongoClient(new MongoClientSettings() { Server = new MongoServerAddress(connectionInfo.Address, connectionInfo.Port) });
            else
                client = new MongoClient(new MongoUrl(connectionInfo.ConnectionString));
            var databases = await client.ListDatabasesAsync();
            MongoDbServer server = new MongoDbServer();
            server.Client = client;
            server.Databases = await databases.ToListAsync();

            return server;
        }

        public async Task<IMongoDatabase> CreateNewDatabase(string databaseName)
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

        public async Task<BsonDocument> ExecuteRawCommand(string databaseName, string command)
        {
            var db = client.GetDatabase(databaseName);
            var result = await db.RunCommandAsync(new BsonDocumentCommand<BsonDocument>(BsonDocument.Parse(command)));
            return result;
        }

        #endregion

        #region Database Admin

        public async Task<List<BsonDocument>> GetCollections(string databaseName)
        {
            var db = client.GetDatabase(databaseName);
            var collections = await db.ListCollectionsAsync();
            var listCollections = await collections.ToListAsync();
            return listCollections;
        }

        public async Task CreateCollection(string databaseName, string collection)
        {
            var db = client.GetDatabase(databaseName);
            await db.CreateCollectionAsync(collection);
        }

        public async Task RenameCollection(string databaseName, string oldName, string newName)
        {
            var db = client.GetDatabase(databaseName);
            await db.RenameCollectionAsync(oldName, newName);
        }

        #endregion

        public async Task<List<BsonDocument>> Find(string databaseName, string collection, string filter, string sort, int? limit, int? skip)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            var result = await mongoCollection.Find(BsonDocument.Parse(filter)).Sort(BsonDocument.Parse(sort)).Limit(limit).Skip(skip).ToListAsync();
            return result;
        }

        public async Task<long> Count(string databaseName, string collection, string filter)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            var result = await mongoCollection.CountAsync(BsonDocument.Parse(filter));
            return result;
        }

        public async Task<BulkWriteResult<BsonDocument>> Insert(string databaseName, string collection, IEnumerable<BsonDocument> documents)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            var result = await mongoCollection.BulkWriteAsync(documents.Select(d => new InsertOneModel<BsonDocument>(d)));
            return result;
        }

    }
}