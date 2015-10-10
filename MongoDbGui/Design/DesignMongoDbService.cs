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
        public async Task<MongoDbServer> ConnectAsync(ConnectionInfo connectionInfo)
        {
            // Use this to create design time data

            MongoDbServer server = new MongoDbServer();
            return server;
        }

        public async Task<IMongoDatabase> CreateNewDatabaseAsync(string databaseName)
        {
            return null;
        }

        public async Task CreateCollectionAsync(string databaseName, string collection)
        {
        }

        public async Task RenameCollectionAsync(string databaseName, string oldName, string newName)
        {
        }

        public async Task<List<BsonDocument>> GetCollectionsAsync(string databaseName)
        {
            // Use this to create design time data

            return new List<BsonDocument>();
        }

        public async Task<BsonDocument> GetCollectionStatsAsync(string databaseName, string collection)
        {
            // Use this to create design time data

            return new BsonDocument();
        }


        public async Task<BsonDocument> ExecuteRawCommandAsync(string databaseName, string command)
        {
            // Use this to create design time data
             
            return new BsonDocument();
        }

        public async Task<List<BsonDocument>> FindAsync(string databaseName, string collection, string filter, string sort, int? limit, int? skip)
        {
            // Use this to create design time data

            return new List<BsonDocument>();
        }

        public async Task<long> CountAsync(string databaseName, string collection, string filter)
        {
            // Use this to create design time data

            return 100;
        }

        public async Task<BulkWriteResult<BsonDocument>> InsertAsync(string databaseName, string collection, IEnumerable<BsonDocument> documents)
        {
            return null;
        }
    }
}