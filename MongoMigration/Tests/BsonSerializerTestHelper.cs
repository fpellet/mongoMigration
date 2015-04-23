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
            using (var buffer = new BsonBuffer())
            {
                using (var bsonWriter = BsonWriter.Create(buffer))
                {
                    BsonSerializer.Serialize(bsonWriter, value);
                }

                return buffer.ToByteArray();
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