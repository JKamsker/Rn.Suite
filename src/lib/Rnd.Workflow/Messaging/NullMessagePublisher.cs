using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rnd.Workflow.Messaging
{
    public class NullMessagePublisher : IMessagePublisher
    {
        public Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            return Task.CompletedTask;
        }

        public Task PublishScheduled<T>(T message, DateTime targetTime, CancellationToken cancellationToken = default) where T : class
        {
            return Task.CompletedTask;
        }
    }
}