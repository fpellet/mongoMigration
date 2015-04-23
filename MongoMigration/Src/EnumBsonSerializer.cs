using System;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public class EnumBsonSerializer : DocumentBsonSerializerBase
    {
        private const string ValueFieldName = "_v";

        protected override void Serialize(BsonWriter bsonWriter, object value)
        {
            bsonWriter.WriteName(ValueFieldName);
            BsonSerializer.Serialize(bsonWriter, typeof(int), (int)value);
        }
        
        protected override object ReadValue(BsonReader bsonReader, Type actualType)
        {
            bsonReader.ReadName(ValueFieldName);

            var value = BsonSerializer.Deserialize(bsonReader, typeof(int));
            return Enum.ToObject(actualType, value);
        }
    }
}