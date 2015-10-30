using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDbGui.Net.Utils
{
    public static class BsonExtensions
    {
        public static string ToJson(this BsonValue value, JsonWriterSettingsExtended settings = null)
        {
            StringBuilder sb = new StringBuilder();
            var serializer = MongoDB.Bson.Serialization.BsonSerializer.LookupSerializer(typeof(BsonDocument));
            MongoDB.Bson.Serialization.BsonSerializationArgs args = default(MongoDB.Bson.Serialization.BsonSerializationArgs);

            using (var stringWriter = new System.IO.StringWriter())
            {
                using (var bsonWriter = new LocalDateTimeJsonWriter(stringWriter, settings))
                {
                    var context = MongoDB.Bson.Serialization.BsonSerializationContext.CreateRoot(bsonWriter, null);
                    args.NominalType = typeof(BsonDocument);
                    serializer.Serialize(context, args, value);
                }
                sb.Append(stringWriter.ToString());
            }

            return sb.ToString();
        }
    }
}
