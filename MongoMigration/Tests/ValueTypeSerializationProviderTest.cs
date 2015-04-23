using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using NFluent;

namespace MongoMigration.Tests
{
    [TestClass]
    public class ValueTypeSerializationProviderTest
    {
        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            var initializer = new InitializeBsonSerializer();
            initializer.Execute();
        }

        [TestMethod]
        public void WhenTypeIsApplicationValueTypeThenReturnCustomSerializer()
        {
            var provider = new ValueTypeSerializationProvider();

            var result = provider.GetSerializer(typeof(StructWithProperty));

            Check.That(result).IsInstanceOf<ValueTypeBsonSerializer>();
        }

        [TestMethod]
        public void WhenTypeIsApplicationEnumThenReturnNull()
        {
            var provider = new ValueTypeSerializationProvider();

            var result = provider.GetSerializer(typeof(TestEnum));

            Check.That(result).IsNull();
        }

        [TestMethod]
        public void WhenTypeIsValueTypeButNotApplicationThenReturnNull()
        {
            var provider = new ValueTypeSerializationProvider();

            var result = provider.GetSerializer(typeof(RuntimeMethodHandle));

            Check.That(result).IsNull();
        }

        [TestMethod]
        public void WhenTypeIsNotValueTypeThenReturnNull()
        {
            var provider = new ValueTypeSerializationProvider();

            var result = provider.GetSerializer(typeof(ValueTypeSerializationProviderTest));

            Check.That(result).IsNull();
        }

        [TestMethod]
        public void WhenSerializeStructWithPropertyThenCanDeserialize()
        {
            var value = new StructWithProperty("ValueA");

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result).IsEqualTo(value);
        }

        [TestMethod]
        public void WhenSerializeStructOfSystemThenNotSupport()
        {
            var value = 5;

            Check.ThatCode(() => BsonSerializerTestHelper.SerializeAndDeserialize(value)).Throws<InvalidOperationException>();
        }

        [TestMethod]
        public void WhenSerializeObjectWithStructOfSystemThenWorks()
        {
            var value = new KeyValuePair<string, int>("essai", 5);

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result).IsEqualTo(value);
        }

        [TestMethod]
        public void WhenSerializeEnumThenNotSupport()
        {
            var provider = new ValueTypeSerializationProvider();

            var result = provider.GetSerializer(typeof(TestEnum));

            Check.That(result).IsNull();
        }

        [TestMethod]
        public void WhenSerializeObjectWithEnumThenWorks()
        {
            var value = new KeyValuePair<string, TestEnum>("essai", TestEnum.EssaiB);

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result).IsEqualTo(value);
        }

        [TestMethod]
        public void WhenSerializeStructInObjectPropertyThenCanDeserialize()
        {
            var value = new StructWrapper(new StructWithProperty("ValueA"));

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result.Value).IsEqualTo(value.Value);
        }

        [TestMethod]
        public void WhenSerializeStructInPropertyThenCanDeserialize()
        {
            var value = new StructWrapperWithGoodType<StructWithProperty>(new StructWithProperty("ValueA"));

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result.Value).IsEqualTo(value.Value);
        }

        [TestMethod]
        public void WhenSerializeStructInPropertyNullableThenCanDeserialize()
        {
            var value = new StructWrapperWithGoodTypeNullable(new StructWithProperty("ValueA"));

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result.Value).IsEqualTo(value.Value);
        }

        [TestMethod]
        public void WhenSerializeStructInPropertyNullableWithEmptyValueThenCanDeserialize()
        {
            var value = new StructWrapperWithGoodTypeNullable(null);

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(value);

            Check.That(result.Value).IsNull();
        }

        [TestMethod]
        public void WhenDeserializeBsonWithExtraDataThenReturnGoodData()
        {
            var value = BsonSerializerTestHelper.Serialize(new StructWithProperty("Essai"));

            var bsonDocument = BsonSerializerTestHelper.Deserialize(value);
            bsonDocument["ExtraName"] = bsonDocument["Value"];
            var eventDataWithExtraData = bsonDocument.ToBson();

            var newValue = BsonSerializerTestHelper.Deserialize<StructWithProperty>(eventDataWithExtraData);
            Check.That(newValue.Value).IsEqualTo("Essai");
        }

        private enum TestEnum
        {
            EssaiA,
            EssaiB
        }

        private struct StructWithProperty
        {
            public StructWithProperty(string value)
                : this()
            {
                Value = value;
            }

            public string Value { get; private set; }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                return obj is StructWithProperty && Equals((StructWithProperty)obj);
            }

            private bool Equals(StructWithProperty other)
            {
                return string.Equals(Value, other.Value);
            }

            public override int GetHashCode()
            {
                return Value != null ? Value.GetHashCode() : 0;
            }

            public static bool operator ==(StructWithProperty left, StructWithProperty right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(StructWithProperty left, StructWithProperty right)
            {
                return !left.Equals(right);
            }
        }

        private class StructWrapper
        {
            public StructWrapper(object value)
            {
                Value = value;
            }

            public object Value { get; private set; }
        }

        private class StructWrapperWithGoodType<T>
        {
            public StructWrapperWithGoodType(T value)
            {
                Value = value;
            }

            public T Value { get; private set; }
        }

        private class StructWrapperWithGoodTypeNullable
        {
            public StructWrapperWithGoodTypeNullable(StructWithProperty? value)
            {
                Value = value;
            }

            public StructWithProperty? Value { get; private set; }
        }
    }
}