using MongoDB.Bson.Serialization;

namespace MongoMigration
{
    public class BsonClassMapWithAssemblyName<T> : BsonClassMap<T>
    {
        public BsonClassMapWithAssemblyName()
        {
            AutoMap();
            var assemblyName = typeof(T).Assembly.GetName().Name;
            SetDiscriminator(typeof(T).Name + "," + assemblyName);
            SetIgnoreExtraElements(true);
        }
    }
}