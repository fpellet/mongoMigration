using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace MongoMigration.Tests
{
    public static class BsonSerializerTestHelper
    {
        public static T SerializeAndDeserialize<T>(T value)
        {
            var data = Serialize(value);

            return Deserialize<T>(data);
        }

        public static byte[] Serialize<T>(T value)
        {
            using (var buffer = new MemoryStream())
            {
                using (var bsonWriter = new BsonBinaryWriter(buffer))
                {
                    BsonSerializer.Serialize(bsonWriter, value);
                }

                return buffer.ToArray();
            }
        }

        public static BsonDocument Deserialize(byte[] data)
        {
            return Deserialize<BsonDocument>(data);
        }

        public static T Deserialize<T>(byte[] data)
        {
            return BsonSerializer.Deserialize<T>(data);
        }
    }
}