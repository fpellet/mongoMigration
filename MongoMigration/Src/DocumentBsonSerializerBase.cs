using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoMigration
{
    public abstract class DocumentBsonSerializerBase : IBsonSerializer
    {
        private readonly IDiscriminatorConvention _discriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));

        public void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            bsonWriter.WriteStartDocument();

            var actualType = value.GetType();
            if (IsNominalType(nominalType, actualType))
            {
                WriteDiscriminator(bsonWriter, actualType);
            }

            Serialize(bsonWriter, value);

            bsonWriter.WriteEndDocument();
        }

        private static bool IsNominalType(Type nominalType, Type actualType)
        {
            return actualType.GetUnderlyingTypeIfNullable() != nominalType.GetUnderlyingTypeIfNullable();
        }

        private void WriteDiscriminator(BsonWriter bsonWriter, Type actualType)
        {
            var discriminator = _discriminatorConvention.GetDiscriminator(typeof(object), actualType);

            bsonWriter.WriteName(_discriminatorConvention.ElementName);
            BsonValueSerializer.Instance.Serialize(bsonWriter, typeof(BsonValue), discriminator, null);
        }

        protected abstract void Serialize(BsonWriter bsonWriter, object value);

        public object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
        {
            return Deserialize(bsonReader, nominalType, nominalType, options);
        }

        public object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            bsonReader.ReadStartDocument();

            if (IsNominalType(nominalType, actualType))
            {
                SkipDiscriminator(bsonReader);
            }

            var value = ReadValue(bsonReader, actualType);

            bsonReader.ReadEndDocument();

            return value;
        }

        private void SkipDiscriminator(BsonReader bsonReader)
        {
            bsonReader.ReadName(_discriminatorConvention.ElementName);
            bsonReader.SkipValue();
        }

        protected abstract object ReadValue(BsonReader bsonReader, Type actualType);

        public IBsonSerializationOptions GetDefaultSerializationOptions()
        {
            return new DocumentSerializationOptions();
        }
    }
}