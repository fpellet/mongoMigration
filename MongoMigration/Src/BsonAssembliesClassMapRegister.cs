namespace MongoMigration
{
    public class BsonAssembliesClassMapRegister
    {
        private readonly ApplicationAssembliesLoader _applicationAssembliesLoader;
        private readonly BsonAssemblyClassMapRegister _classMapRegister;

        public BsonAssembliesClassMapRegister()
            : this(new ApplicationAssembliesLoader(), new BsonAssemblyClassMapRegister())
        {
        }

        public BsonAssembliesClassMapRegister(ApplicationAssembliesLoader applicationAssembliesLoader, BsonAssemblyClassMapRegister classMapRegister)
        {
            _applicationAssembliesLoader = applicationAssembliesLoader;
            _classMapRegister = classMapRegister;
        }

        public void RegisterAllTypesOfAllApplicationAssemblies()
        {
            foreach (var assembly in _applicationAssembliesLoader.LoadAllAssemblies())
            {
                _classMapRegister.RegisterAllTypesOf(assembly);
            }
        }
    }
}