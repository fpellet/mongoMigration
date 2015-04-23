using System;
using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public class ValueTypeSerializationProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            return IsApplicationValueType(type) ? new ValueTypeBsonSerializer() : null;
        }

        private static bool IsApplicationValueType(Type type)
        {
            return type.IsValueType && type.FullName.StartsWith("MongoMigration", StringComparison.Ordinal) && !type.IsEnum;
        }
    }
}