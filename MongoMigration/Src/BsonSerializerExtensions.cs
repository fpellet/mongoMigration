using System.IO;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public static class BsonSerializerExtensions
    {
        public static byte[] Serialize<T>(T value)
        {
            using (var buffer = new MemoryStream())
            {
                using (var bsonWriter = new BsonBinaryWriter(buffer))
                {
                    BsonSerializer.Serialize(bsonWriter, value, args: new BsonSerializationArgs(typeof(T), false, false));
                }

                return buffer.ToArray();
            }
        }
    }
}
