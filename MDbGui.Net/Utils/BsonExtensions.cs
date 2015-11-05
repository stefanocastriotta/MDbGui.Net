using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
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
            var serializer = MongoDB.Bson.Serialization.BsonSerializer.LookupSerializer(typeof(BsonValue));
            MongoDB.Bson.Serialization.BsonSerializationArgs args = default(MongoDB.Bson.Serialization.BsonSerializationArgs);

            using (var stringWriter = new System.IO.StringWriter())
            {
                using (var bsonWriter = new LocalDateTimeJsonWriter(stringWriter, settings))
                {
                    var context = MongoDB.Bson.Serialization.BsonSerializationContext.CreateRoot(bsonWriter, null);
                    args.NominalType = typeof(BsonValue);
                    serializer.Serialize(context, args, value);
                }
                sb.Append(stringWriter.ToString());
            }

            return sb.ToString();
        }

        public static TNominalType Deserialize<TNominalType>(this string json, Action<BsonDeserializationContext.Builder> configurator = null)
        {
            using (var bsonReader = new JsonReader(json))
            {
                try
                {
                    return BsonSerializer.Deserialize<TNominalType>(bsonReader, configurator);
                }
                catch (Exception ex)
                {
                    var _bufferProp = bsonReader.GetType().GetField("_buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var _buffer = _bufferProp.GetValue(bsonReader);
                    var _positionProp = _buffer.GetType().GetProperty("Position", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    int Position = (int)_positionProp.GetValue(_buffer);
                    throw new BsonParseException(ex, Position);
                }
            }
        }

        public class BsonParseException : Exception
        {
            public int Position { get; set; }

            public BsonParseException(Exception ex, int position) : base(ex.Message + Environment.NewLine + "Position: " + position, ex)
            {
                Position = position;
            }
        }
    }
}
