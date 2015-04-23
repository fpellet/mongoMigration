using System;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public class BsonAssemblyClassMapRegister
    {
        private static readonly char[] SpecialNameFirstLetters = { '<', '_' };

        public void RegisterAllTypesOf(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(IsFinalType).Where(IsNotSpecialName))
            {
                RegisterType(type);
            }
        }

        private static bool IsFinalType(Type type)
        {
            return !type.IsAbstract && !type.IsGenericType && !type.IsInterface;
        }

        private static bool IsNotSpecialName(Type type)
        {
            var name = type.Name;
            return !SpecialNameFirstLetters.Contains(name[0]);
        }

        private static void RegisterType(Type type)
        {
            if (BsonClassMap.IsClassMapRegistered(type))
            {
                return;
            }

            RegisterInBsonSerializer(type);
        }

        private static void RegisterInBsonSerializer(Type type)
        {
            BsonClassMap.RegisterClassMap(CreateGenericSerializer(typeof(BsonClassMapWithAssemblyName<>), type));
        }

        private static BsonClassMap CreateGenericSerializer(Type serializerTypeDefinition, params Type[] typeArguments)
        {
            var serializerType = serializerTypeDefinition.MakeGenericType(typeArguments);
            return (BsonClassMap)Activator.CreateInstance(serializerType);
        }
    }
}