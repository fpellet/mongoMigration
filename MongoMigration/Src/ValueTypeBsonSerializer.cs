using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public class ValueTypeBsonSerializer : DocumentBsonSerializerBase 
    {
        private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
        
        protected override void Serialize(BsonWriter bsonWriter, object value)
        {
            var actualType = value.GetType();

            WriteAllProperties(bsonWriter, value, actualType);
        }

        private static void WriteAllProperties(BsonWriter bsonWriter, object value, Type actualType)
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

        protected override object ReadValue(BsonReader bsonReader, Type actualType)
        {
            var obj = Activator.CreateInstance(actualType);

            while (IsDocumentEnd(bsonReader))
            {
                var fieldName = bsonReader.ReadName();

                TryFillProperty(bsonReader, actualType, fieldName, obj);
            }

            return obj;
        }

        private static bool IsDocumentEnd(BsonReader bsonReader)
        {
            return bsonReader.ReadBsonType() != BsonType.EndOfDocument;
        }

        private static void TryFillProperty(BsonReader bsonReader, Type actualType, string name, object obj)
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

        private static object ReadValueOfProperty(BsonReader bsonReader, Type typeOfValue)
        {
            return BsonSerializer.Deserialize(bsonReader, typeOfValue);
        }
    }
}