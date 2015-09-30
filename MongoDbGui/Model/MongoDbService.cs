using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDbGui.Model
{
    public class MongoDbService : IMongoDbService
    {
        public async Task<MongoDbServer> Connect(ConnectionInfo connectionInfo)
        {
            MongoClient client;
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

        public async Task<List<BsonDocument>> GetCollections(MongoClient client, string databaseName)
        {
            var db = client.GetDatabase(databaseName);
            var collections = await db.ListCollectionsAsync();
            var listCollections = await collections.ToListAsync();
            return listCollections;
        }

        public async Task<BsonDocument> ExecuteRawCommand(MongoClient client, string databaseName, string command)
        {
            var db = client.GetDatabase(databaseName);
            var result = await db.RunCommandAsync(new BsonDocumentCommand<BsonDocument>(BsonDocument.Parse(command)));
            return result;
        }

        public async Task<List<BsonDocument>> Find(MongoClient client, string databaseName, string collection, string find, string sort, int? size, int? skip)
        {
            var db = client.GetDatabase(databaseName);
            var mongoCollection = db.GetCollection<BsonDocument>(collection);
            var result = await mongoCollection.Find(BsonDocument.Parse(find)).Sort(BsonDocument.Parse(sort)).Limit(size).Skip(skip).ToListAsync();
            return result;
        }
    }
}