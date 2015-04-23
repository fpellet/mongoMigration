using System;
using System.Globalization;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public class DecimalBsonSerializer : DocumentBsonSerializerBase<decimal>
    {
        private const string ValueFieldName = "_v";

        protected override void Serialize(IBsonWriter bsonWriter, decimal value)
        {
            bsonWriter.WriteName(ValueFieldName);

            BsonSerializer.Serialize(bsonWriter, typeof(string), value.ToString(CultureInfo.InvariantCulture));
        }

        protected override decimal ReadValue(IBsonReader bsonReader, Type actualType)
        {
            bsonReader.ReadName(ValueFieldName);

            var valueFormatted = (string)BsonSerializer.Deserialize(bsonReader, typeof(string));
            return decimal.Parse(valueFormatted, CultureInfo.InvariantCulture);
        }
    }
}