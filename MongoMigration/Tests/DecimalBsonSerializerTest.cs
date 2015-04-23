using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson.Serialization;
using NFluent;

namespace MongoMigration.Tests
{
    [TestClass]
    public class DecimalBsonSerializerTest
    {
        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            var initializer = new InitializeBsonSerializer();
            initializer.Execute();
        }

        [TestMethod]
        public void WhenSerializeDecimalThenCanDeserialize()
        {
            const decimal Value = 5.12M;

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(Value);

            Check.That(result).IsEqualTo(Value);
        }

        [TestMethod]
        public void WhenSerializeDecimalInObjectPropertyThenCanDeserialize()
        {
            var value = new DecimalWrapper(5.12M);

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result.Value).IsEqualTo(value.Value);
        }

        [TestMethod]
        public void WhenSerializeDecimalPropertyThenCanDeserialize()
        {
            var value = new DecimalWrapperWithGoodType(5.12M);

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result.Value).IsEqualTo(value.Value);
        }

        [TestMethod]
        public void WhenSerializeDecimalPropertyNullableThenCanDeserialize()
        {
            var value = new DecimalWrapperWithNullableType(5.12M);

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result.Value).IsEqualTo(value.Value);
        }

        [TestMethod]
        public void WhenSerializeEmptyDecimalPropertyNullableThenCanDeserialize()
        {
            var value = new DecimalWrapperWithNullableType(null);

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result.Value).IsNull();
        }

        [TestMethod]
        public void GivenJsonSerializeWithDecimalAsDiscriminantWhenDeserializeThenReturnGoodValue()
        {
            var json = "{" +
                       "\"Value\" : {" +
                       "  \"_t\" : \"Decimal\"," +
                       "  \"_v\" : \"5.23\"" +
                       " }," +
                       "}";
            var result = BsonSerializer.Deserialize<DecimalWrapper>(json);

            Check.That(result.Value).IsEqualTo(5.23M);
        }

        [TestMethod]
        public void GivenJsonSerializeWithoutDecimalAsDiscriminantWhenDeserializeThenReturnGoodValue()
        {
            var json = "{" +
                       "\"Value\" : {" +
                       "  \"_v\" : \"5.23\"" +
                       " }," +
                       "}";
            var result = BsonSerializer.Deserialize<DecimalWrapperWithGoodType>(json);

            Check.That(result.Value).IsEqualTo(5.23M);
        }

        private class DecimalWrapper
        {
            public DecimalWrapper(object value)
            {
                Value = value;
            }

            public object Value { get; private set; }
        }

        private class DecimalWrapperWithGoodType
        {
            public DecimalWrapperWithGoodType(decimal value)
            {
                Value = value;
            }

            public decimal Value { get; private set; }
        }

        private class DecimalWrapperWithNullableType
        {
            public DecimalWrapperWithNullableType(decimal? value)
            {
                Value = value;
            }

            public decimal? Value { get; private set; }
        }
    }
}