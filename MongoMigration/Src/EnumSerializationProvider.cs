using System;
using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public class EnumSerializationProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            return type.IsEnum && type.FullName.StartsWith("MongoMigration", StringComparison.Ordinal) ? CreateGenericSerializer(typeof(EnumBsonSerializer<>), type) : null;
        }

        protected virtual IBsonSerializer CreateGenericSerializer(Type serializerTypeDefinition, params Type[] typeArguments)
        {
            var serializerType = serializerTypeDefinition.MakeGenericType(typeArguments);
            return (IBsonSerializer)Activator.CreateInstance(serializerType);
        }
    }
}