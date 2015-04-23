using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using NFluent;

namespace MongoMigration.Tests
{
    [TestClass]
    public class InitializeBsonSerializerTest
    {
        private InitializeBsonSerializer _command;

        [TestInitialize]
        public void Initialize()
        {
            _command = new InitializeBsonSerializer();
        }

        [TestMethod]
        public void WhenSerializeDateTimeThenPreserveTimeZone()
        {
            _command.Execute();

            var dateTime = DateTime.Today;

            var result = BsonSerializerTestHelper.SerializeAndDeserialize(new KeyValuePair<string, DateTime>("essai", dateTime)).Value;

            Check.That(result).IsEqualTo(dateTime);
        }

        [TestMethod]
        public void WhenSerializeWithExtraDataThenCanDeserialize()
        {
            _command.Execute();

            var eventData = BsonSerializerTestHelper.Serialize(new EssaiEventdfgdfgfdg { Name = "Essai" });

            var bsonDocument = BsonSerializerTestHelper.Deserialize(eventData);
            bsonDocument["ExtraName"] = bsonDocument["Name"];
            var eventDataWithExtraData = bsonDocument.ToBson();

            var newEvent = BsonSerializerTestHelper.Deserialize<EssaiEventdfgdfgfdg>(eventDataWithExtraData);

            Check.That(newEvent.Name).IsEqualTo("Essai");
        }

        private class EssaiEventdfgdfgfdg
        {
            public string Name { get; set; }
        }
    }
}
