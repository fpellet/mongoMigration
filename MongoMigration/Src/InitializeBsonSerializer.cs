﻿using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoMigration
{
    public class InitializeBsonSerializer
    {
        private static readonly object InitializeLock = new object();

        private static volatile bool _isInitialized;

        private readonly BsonAssembliesClassMapRegister _assembliesClassMapRegister;

        public InitializeBsonSerializer() :
            this(new BsonAssembliesClassMapRegister())
        {
        }

        public InitializeBsonSerializer(BsonAssembliesClassMapRegister assembliesClassMapRegister)
        {
            _assembliesClassMapRegister = assembliesClassMapRegister;
        }

        public void Execute()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            lock (InitializeLock)
            {
                if (_isInitialized)
                {
                    return;
                }

                EnableLocalTimeZoneOnDateTime();
                EnableValueTypeConverter();
                EnableDecimalConverter();
                EnableEnumConverter();

                _assembliesClassMapRegister.RegisterAllTypesOfAllApplicationAssemblies();

                _isInitialized = true;
            }
        }

        private static void EnableDecimalConverter()
        {
            Try(() => BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalBsonSerializer()));
            Try(() => BsonClassMap.RegisterClassMap<decimal>(o => o.AutoMap()));
        }

        private static void EnableValueTypeConverter()
        {
            BsonSerializer.RegisterSerializationProvider(new ValueTypeSerializationProvider());
        }

        private static void EnableLocalTimeZoneOnDateTime()
        {
            Try(() => BsonSerializer.RegisterSerializer(typeof(DateTime), new DateTimeSerializer(DateTimeSerializationOptions.LocalInstance)));
        }

        private void EnableEnumConverter()
        {
            BsonSerializer.RegisterSerializationProvider(new EnumSerializationProvider());
        }

        private static void Try(Action action)
        {
            try
            {
                action();
            }
            catch (BsonSerializationException)
            {
            }
        }
    }
}
