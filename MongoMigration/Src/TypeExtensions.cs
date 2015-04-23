using System;

namespace MongoMigration
{
    public static class TypeExtensions
    {
        public static Type GetUnderlyingTypeIfNullable(this Type type)
        {
            return type.IsNullable() ? Nullable.GetUnderlyingType(type) : type;
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}