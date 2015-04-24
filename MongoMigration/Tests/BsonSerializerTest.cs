using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using NFluent;

namespace MongoMigration.Tests
{
    [TestClass]
    public class BsonSerializerTest
    {
        [TestMethod]
        public void WhenSerializeWithGoodTypeThenNotWriteDiscriminator()
        {
            var value = new ClassA();

            var result = ToJson(value);

            Check.That(result).IsEqualTo(@"{ }");
        }

        [TestMethod]
        public void WhenSerializeWithSubTypeThenWriteDiscriminator()
        {
            var value = new ClassA();

            var result = ToJson<object>(value);

            Check.That(result).IsEqualTo(@"{ ""_t"" : ""ClassA,MongoMigration"" }");
        }

        private static string ToJson<TNominal>(TNominal value)
        {
            var serializer = new BsonSerializer<TNominal>();
            var serialized = serializer.Serialize(value);

            return BsonSerializerTestHelper.Deserialize<BsonDocument>(serialized).ToJson();
        }

        private class ClassA
        {
        }
    }
}
