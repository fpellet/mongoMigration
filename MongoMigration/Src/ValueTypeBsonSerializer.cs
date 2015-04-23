using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public class ValueTypeBsonSerializer<TStruct> : DocumentBsonSerializerBase<TStruct>
    {
        private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;

        protected override void Serialize(IBsonWriter bsonWriter, TStruct value)
        {
            var actualType = value.GetType();

            WriteAllProperties(bsonWriter, value, actualType);
        }

        private static void WriteAllProperties(IBsonWriter bsonWriter, object value, Type actualType)
        {
            foreach (var property in GetAllProperties(actualType))
            {
                bsonWriter.WriteName(property.Name);
                BsonSerializer.Serialize(bsonWriter, property.PropertyType, property.GetValue(value, null));
            }
        }

        private static IEnumerable<PropertyInfo> GetAllProperties(Type actualType)
        {
            return actualType.GetProperties(BindingFlags).Where(o => o.CanWrite);
        }

        protected override TStruct ReadValue(IBsonReader bsonReader, Type actualType)
        {
            var obj = Activator.CreateInstance(actualType);

            while (IsDocumentEnd(bsonReader))
            {
                var fieldName = bsonReader.ReadName();

                TryFillProperty(bsonReader, actualType, fieldName, obj);
            }

            return (TStruct)obj;
        }

        private static bool IsDocumentEnd(IBsonReader bsonReader)
        {
            return bsonReader.ReadBsonType() != BsonType.EndOfDocument;
        }

        private static void TryFillProperty(IBsonReader bsonReader, Type actualType, string name, object obj)
        {
            var property = TryGetProperty(actualType, name);
            if (property != null)
            {
                var value = ReadValueOfProperty(bsonReader, property.PropertyType);
                property.SetValue(obj, value, null);
            }
            else
            {
                bsonReader.SkipValue();
            }
        }

        private static PropertyInfo TryGetProperty(Type actualType, string name)
        {
            return actualType.GetProperty(name, BindingFlags);
        }

        private static object ReadValueOfProperty(IBsonReader bsonReader, Type typeOfValue)
        {
            return BsonSerializer.Deserialize(bsonReader, typeOfValue);
        }
    }
}