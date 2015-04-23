using System;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public class EnumBsonSerializer<TEnum> : DocumentBsonSerializerBase<TEnum>
    {
        private const string ValueFieldName = "_v";

        protected override void Serialize(IBsonWriter bsonWriter, TEnum value)
        {
            bsonWriter.WriteName(ValueFieldName);
            bsonWriter.WriteInt32(Convert.ToInt32(value));
        }

        protected override TEnum ReadValue(IBsonReader bsonReader, Type actualType)
        {
            bsonReader.ReadName(ValueFieldName);

            var value = BsonSerializer.Deserialize(bsonReader, typeof(int));
            return (TEnum)Enum.ToObject(actualType, value);
        }
    }
}