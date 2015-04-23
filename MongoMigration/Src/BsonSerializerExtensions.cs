using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public static class BsonSerializerExtensions
    {
        public static byte[] Serialize<T>(T value)
        {
            using (var buffer = new BsonBuffer())
            {
                using (var bsonWriter = BsonWriter.Create(buffer))
                {
                    BsonSerializer.Serialize(bsonWriter, value);
                }

                return buffer.ToByteArray();
            }
        }
    }
}
