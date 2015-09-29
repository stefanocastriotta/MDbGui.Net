﻿using MongoDB.Bson;
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
    }
}