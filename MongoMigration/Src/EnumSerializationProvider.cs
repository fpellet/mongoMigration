using System;
using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public class EnumSerializationProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            return type.IsEnum && type.FullName.StartsWith("MongoMigration", StringComparison.Ordinal) ? new EnumBsonSerializer() : null;
        }
    }
}