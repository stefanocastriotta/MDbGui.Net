using System;
using MongoDbGui.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDbGui.Design
{
    public class DesignMongoDbService : IMongoDbService
    {
        public async Task<MongoDbServer> Connect(ConnectionInfo connectionInfo)
        {
            // Use this to create design time data

            MongoDbServer server = new MongoDbServer();
            return server;
        }

        public async Task<IMongoDatabase> CreateNewDatabase(MongoClient client, string databaseName, string collection)
        {
            return null;
        }

        public async void CreateCollection(MongoClient client, string databaseName, string collection)
        {
        }

        public async Task<List<BsonDocument>> GetCollections(MongoClient client, string databaseName)
        {
            // Use this to create design time data

            return new List<BsonDocument>();
        }

        public async Task<BsonDocument> ExecuteRawCommand(MongoClient client, string databaseName, string command)
        {
            // Use this to create design time data
             
            return new BsonDocument();
        }

        public async Task<List<BsonDocument>> Find(MongoClient client, string databaseName, string collection, string filter, string sort, int? limit, int? skip)
        {
            // Use this to create design time data

            return new List<BsonDocument>();
        }

        public async Task<long> Count(MongoClient client, string databaseName, string collection, string filter)
        {
            // Use this to create design time data

            return 100;
        }
    }
}