using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoMigration
{
    public class ObjectBsonSerializerWithEnumSupport : ObjectSerializer
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            if (value != null && value.GetType().IsEnum)
            {
                var serializer = BsonSerializer.LookupSerializer(value.GetType());
                serializer.Serialize(context, args, value);
                return;
            }
            
            base.Serialize(context, args, value);
        }
    }
}