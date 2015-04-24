using System;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoMigration
{
    public abstract class DocumentBsonSerializerBase<TValueType> : IBsonSerializer<TValueType>, IBsonPolymorphicSerializer
    {
        private readonly IDiscriminatorConvention _discriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(TValueType));

        public Type ValueType
        {
            get { return typeof(TValueType); }
        }

        public bool IsDiscriminatorCompatibleWithObjectSerializer
        {
            get { return true; }
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            Serialize(context, args, (TValueType)value);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TValueType value)
        {
            var bsonWriter = context.Writer;

            bsonWriter.WriteStartDocument();

            WriteDiscriminator(context, args.NominalType, value.GetType());

            Serialize(bsonWriter, value);

            bsonWriter.WriteEndDocument();
        }

        private bool IsNominalType(Type nominalType, Type actualType)
        {
            nominalType = nominalType ?? ValueType;

            return actualType.GetUnderlyingTypeIfNullable() != nominalType.GetUnderlyingTypeIfNullable();
        }

        private void WriteDiscriminator(BsonSerializationContext context, Type nominalType, Type actualType)
        {
            var discriminator = _discriminatorConvention.GetDiscriminator(nominalType, actualType);
            if (discriminator == null)
            {
                return;
            }

            context.Writer.WriteName(_discriminatorConvention.ElementName);
            BsonValueSerializer.Instance.Serialize(context, discriminator);
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

        private TValueType Deserialize(IBsonReader bsonReader, Type nominalType, Type actualType)
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