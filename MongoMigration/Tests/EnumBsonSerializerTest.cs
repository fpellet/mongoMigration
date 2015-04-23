using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson.Serialization;
using NFluent;

namespace MongoMigration.Tests
{
    [TestClass]
    public class EnumBsonSerializerTest
    {
        private enum EnumB
        {
            ValueC = 3,
            ValueD = 4
        }

        private enum EnumA
        {
            ValueA = 1,
            ValueB = 2
        }

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            var initializer = new InitializeBsonSerializer();
            initializer.Execute();
        }

        [TestMethod]
        public void WhenTypeIsApplicationEnumThenReturnCustomSerializer()
        {
            var provider = new EnumSerializationProvider();

            var result = provider.GetSerializer(typeof(EnumB));

            Check.That(result).IsInstanceOf<EnumBsonSerializer<EnumB>>();
        }

        [TestMethod]
        public void WhenTypeIsEnumButNotApplicationThenReturnNull()
        {
            var provider = new EnumSerializationProvider();

            var result = provider.GetSerializer(typeof(HttpStatusCode));

            Check.That(result).IsNull();
        }

        [TestMethod]
        public void WhenTypeIsNotEnumThenReturnNull()
        {
            var provider = new EnumSerializationProvider();

            var result = provider.GetSerializer(typeof(EnumBsonSerializerTest));

            Check.That(result).IsNull();
        }

        [TestMethod]
        public void WhenSerializeEnumThenCanDeserialize()
        {
            var value = new TyppedEnumWrapper<EnumB>(EnumB.ValueC);

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result.Value).IsEqualTo(value.Value);
        }

        [TestMethod]
        public void WhenSerializeEnumInObjectPropertyThenCanDeserialize()
        {
            var value = new EnumWrapper(EnumB.ValueC);

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result.Value).IsEqualTo(value.Value);
        }

        [TestMethod]
        public void WhenSerializeInvalidEnumValueInObjectPropertyThenCanDeserialize()
        {
            var value = new EnumWrapper((EnumB)99);

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result.Value).IsEqualTo(value.Value);
        }

        [TestMethod]
        public void GivenOldValueSerializeWhenDeserializeEnumValueInObjectPropertyThenReturnGoodValue()
        {
            var oldValue = @"{ ""Value"" : { ""_t"" : ""EnumA,MongoMigration"", ""_v"" : 1 } }";

            var result = BsonSerializer.Deserialize<EnumWrapper>(oldValue);

            Check.That(result.Value).IsInstanceOf<EnumA>();
        }

        [TestMethod]
        public void WhenSerializeEnumNullableThenCanDeserialize()
        {
            var value = new NullableTyppedEnumWrapper(EnumB.ValueC);

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result.Value).IsEqualTo(value.Value);
        }

        [TestMethod]
        public void WhenSerializeEnumNullableEmptyThenCanDeserialize()
        {
            var value = new NullableTyppedEnumWrapper(null);

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result.Value).IsNull();
        }

        private class NullableTyppedEnumWrapper
        {
            public NullableTyppedEnumWrapper(EnumB? value)
            {
                Value = value;
            }

            public EnumB? Value { get; private set; }
        }

        private class TyppedEnumWrapper<T>
        {
            public TyppedEnumWrapper(T value)
            {
                Value = value;
            }

            public T Value { get; private set; }
        }

        private class EnumWrapper
        {
            public EnumWrapper(object value)
            {
                Value = value;
            }

            public object Value { get; private set; }
        }
    }
}