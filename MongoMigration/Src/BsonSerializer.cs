using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public class BsonSerializer<TNominal> : ISerializer<TNominal>
    {
        public BsonSerializer()
        {
            new InitializeBsonSerializer().Execute();
        }

        public byte[] Serialize(TNominal element)
        {
            return BsonSerializerExtensions.Serialize(element);
        }

        public TNominal Deserialize(byte[] data)
        {
            return BsonSerializer.Deserialize<TNominal>(data);
        }
    }
}