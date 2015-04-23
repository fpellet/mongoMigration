using System;
using System.Globalization;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public class DecimalBsonSerializer : DocumentBsonSerializerBase
    {
        private const string ValueFieldName = "_v";

        protected override void Serialize(BsonWriter bsonWriter, object value)
        {
            bsonWriter.WriteName(ValueFieldName);

            var decimalValue = (decimal)value;
            BsonSerializer.Serialize(bsonWriter, typeof(string), decimalValue.ToString(CultureInfo.InvariantCulture));
        }

        protected override object ReadValue(BsonReader bsonReader, Type actualType)
        {
            bsonReader.ReadName(ValueFieldName);

            var valueFormatted = (string)BsonSerializer.Deserialize(bsonReader, typeof(string));
            return decimal.Parse(valueFormatted, CultureInfo.InvariantCulture);
        }
    }
}