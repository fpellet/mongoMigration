using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson.Serialization;
using NFluent;

namespace MongoMigration.Tests
{
    [TestClass]
    public class BsonAssemblyClassMapRegisterTest
    {
        [TestMethod]
        public void WhenExecuteThenRegisterAllFinalTypeOfAssembly()
        {
            var register = new BsonAssemblyClassMapRegister();

            register.RegisterAllTypesOf(GetType().Assembly);

            CheckThatClassMapRegistered<TypeA>().IsTrue();
            CheckThatClassMapRegistered<TypeB>().IsTrue();
        }

        [TestMethod]
        public void WhenExecuteThenNotRegisterInterface()
        {
            var register = new BsonAssemblyClassMapRegister();

            register.RegisterAllTypesOf(GetType().Assembly);

            CheckThatClassMapRegistered<InterfaceType>().IsFalse();
        }

        [TestMethod]
        public void WhenExecuteThenNotRegisterAbstractClass()
        {
            var register = new BsonAssemblyClassMapRegister();

            register.RegisterAllTypesOf(GetType().Assembly);

            CheckThatClassMapRegistered<AbstractType>().IsFalse();
        }

        private ICheck<bool> CheckThatClassMapRegistered<T>()
        {
            return Check.That(BsonClassMap.IsClassMapRegistered(typeof(T)));
        }

        [TestMethod]
        public void WhenExecuteSeveralTimesThenNotRaiseException()
        {
            var register = new BsonAssemblyClassMapRegister();

            register.RegisterAllTypesOf(GetType().Assembly);

            Check.ThatCode(() => register.RegisterAllTypesOf(GetType().Assembly)).DoesNotThrow();
        }

        private abstract class AbstractType
        {
        }

        private interface InterfaceType
        {
        }

        private class TypeA
        {
        }

        private enum TypeB
        {
        }
    }
}