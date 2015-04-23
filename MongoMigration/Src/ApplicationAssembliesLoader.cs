using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MongoMigration
{
    public class ApplicationAssembliesLoader
    {
        private const string ApplicationNamespace = "MongoMigration";

        public IEnumerable<Assembly> LoadAllAssemblies()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(o => IsApplicationAssembly(o.FullName));

            foreach (var loadedAssembly in loadedAssemblies)
            {
                yield return loadedAssembly;

                foreach (var assemblyLoaded in LoadReferencedAssemblies(loadedAssembly))
                {
                    yield return assemblyLoaded;
                }
            }
        }

        private IEnumerable<Assembly> LoadReferencedAssemblies(Assembly assemblyReference)
        {
            return assemblyReference.GetReferencedAssemblies()
                                    .Where(o => IsApplicationAssembly(o.FullName))
                                    .SelectMany(LoadAssemblyWithReferences);
        }

        private IEnumerable<Assembly> LoadAssemblyWithReferences(AssemblyName assemblyName)
        {
            var assembly = Assembly.Load(assemblyName);
            yield return assembly;

            foreach (var assemblyLoaded in LoadReferencedAssemblies(assembly))
            {
                yield return assemblyLoaded;
            }
        }

        private static bool IsApplicationAssembly(string fullName)
        {
            return fullName.StartsWith(ApplicationNamespace, StringComparison.Ordinal);
        }
    }
}