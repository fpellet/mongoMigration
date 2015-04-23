using System;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoMigration
{
    public abstract class DocumentBsonSerializerBase<TValueType> : IBsonSerializer<TValueType>
    {
        private readonly IDiscriminatorConvention _discriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(object));

        public Type ValueType
        {
            get { return typeof(TValueType); }
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            Serialize(context, args, (TValueType)value);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TValueType value)
        {
            var bsonWriter = context.Writer;

            bsonWriter.WriteStartDocument();

            var actualType = value.GetType();
            if (IsNominalType(args.NominalType, actualType))
            {
                WriteDiscriminator(context, args, actualType);
            }

            Serialize(bsonWriter, value);

            bsonWriter.WriteEndDocument();
        }

        private static bool IsNominalType(Type nominalType, Type actualType)
        {
            return actualType.GetUnderlyingTypeIfNullable() != nominalType.GetUnderlyingTypeIfNullable();
        }

        private void WriteDiscriminator(BsonSerializationContext context, BsonSerializationArgs args, Type actualType)
        {
            var discriminator = _discriminatorConvention.GetDiscriminator(typeof(object), actualType);

            context.Writer.WriteName(_discriminatorConvention.ElementName);
            BsonValueSerializer.Instance.Serialize(context, args, discriminator);
        }

        protected abstract void Serialize(IBsonWriter bsonWriter, TValueType value);

        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }

        public TValueType Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context.Reader, args.NominalType, ValueType);
        }

        public TValueType Deserialize(IBsonReader bsonReader, Type nominalType, Type actualType)
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

        private void SkipDiscriminator(IBsonReader bsonReader)
        {
            bsonReader.ReadName(_discriminatorConvention.ElementName);
            bsonReader.SkipValue();
        }

        protected abstract TValueType ReadValue(IBsonReader bsonReader, Type actualType);
    }
}