using System;
using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public class BsonClassMapWithAssemblyName : BsonClassMap
    {
        public BsonClassMapWithAssemblyName(Type classType)
            : base(classType)
        {
            AutoMap();
            var assemblyName = classType.Assembly.GetName().Name;
            SetDiscriminator(classType.Name + "," + assemblyName);
            SetIgnoreExtraElements(true);
        }
    }
}