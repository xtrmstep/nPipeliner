﻿using System.Threading;
using System.Threading.Tasks;
using Bruteflow.Blocks;
using Bruteflow.Kafka.Producers;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Tests.Pipeline
{
    public class TestPipe : IPipe<JObject>
    {
        private readonly IKafkaProducer<string, JObject> _producer;

        /// <inheritdoc />
        public HeadBlock<JObject> Head { get; } = new HeadBlock<JObject>();

        public TestPipe(IProducerFactory<string, JObject> producerFactory)
        {
            _producer = producerFactory.CreateProducer();
            Head
                .Process(AddProperty)
                .Action(Send);
        }
        
        private static Task<JObject> AddProperty(CancellationToken cancellationToken, JObject json, PipelineMetadata metadata)
        {
            json.Add(new JProperty("testProperty", 1));
            return Task.FromResult(json);
        }
        
        private Task Send(CancellationToken cancellationToken, JObject json, PipelineMetadata metadata)
        {
            return _producer.ProduceAsync("key", json);
        }
    }
}