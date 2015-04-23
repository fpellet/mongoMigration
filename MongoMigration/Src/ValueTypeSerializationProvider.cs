using System;
using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public class ValueTypeSerializationProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            return IsApplicationValueType(type) ? CreateGenericSerializer(typeof(ValueTypeBsonSerializer<>), type) : null;
        }

        private static bool IsApplicationValueType(Type type)
        {
            return type.IsValueType && type.FullName.StartsWith("MongoMigration", StringComparison.Ordinal) && !type.IsEnum;
        }

        protected virtual IBsonSerializer CreateGenericSerializer(Type serializerTypeDefinition, params Type[] typeArguments)
        {
            var serializerType = serializerTypeDefinition.MakeGenericType(typeArguments);
            return (IBsonSerializer)Activator.CreateInstance(serializerType);
        }
    }
}