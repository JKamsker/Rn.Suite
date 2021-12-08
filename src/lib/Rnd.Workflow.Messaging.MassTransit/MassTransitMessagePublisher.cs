using System;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

namespace Rnd.Workflow.Messaging.MassTransit
{
    public class MassTransitMessagePublisher : IMessagePublisher
    {
        private readonly IBus _bus;
        private readonly IMessageScheduler _messageScheduler;

        public MassTransitMessagePublisher(IBus bus, IMessageScheduler messageScheduler)
        {
            _bus = bus;
            _messageScheduler = messageScheduler;
        }

        public Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            //_messageScheduler.SchedulePublish()
            return _bus.Publish(message, cancellationToken);
        }

        public async Task PublishScheduled<T>(T message, DateTime targetTime, CancellationToken cancellationToken = default) where T : class
        {
            await _messageScheduler.SchedulePublish(targetTime, message, cancellationToken);
        }
    }
}