using MongoDB.Bson;

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
            var serialize = new BsonSerializer<T>();
            return serialize.Serialize(value);
        }

        public static BsonDocument Deserialize(byte[] data)
        {
            return Deserialize<BsonDocument>(data);
        }

        public static T Deserialize<T>(byte[] data)
        {
            var serialize = new BsonSerializer<T>();
            return serialize.Deserialize(data);
        }
    }
}