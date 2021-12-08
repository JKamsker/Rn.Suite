using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using MassTransit;

namespace Rnd.Workflow.Messaging.MassTransit
{
    public class MassTransitMessageConsumer<TMessage>
        : IConsumer<TMessage>
        where TMessage : class
    {
        private readonly IMessageConsumer<TMessage> _consumer;

        public MassTransitMessageConsumer(IMessageConsumer<TMessage> consumer)
        {
            _consumer = consumer;
        }

        public Task Consume(ConsumeContext<TMessage> context)
        {
            return _consumer.ConsumeAsync(context.Message);
        }
    }

    public class MessageRedistributor<TMessage>
        : IMessageDistributor<TMessage>, IMessageConsumer<TMessage>
        where TMessage : class
    {
        private readonly List<IMessageConsumer<TMessage>> _consumers = new();

        public MessageRedistributor()
        {
        }

        public async Task ConsumeAsync(TMessage message)
        {
            foreach (var consumer in _consumers)
            {
                await consumer.ConsumeAsync(message);
            }
        }

        public Task SubscribeAsync(IMessageConsumer<TMessage> consumer)
        {
            _consumers.Add(consumer);
            return Task.CompletedTask;
        }

        public Task UnSubscribeAsync(IMessageConsumer<TMessage> consumer)
        {
            _consumers.RemoveAll(x => ReferenceEquals(x, consumer));
            return Task.CompletedTask;
        }
    }
}