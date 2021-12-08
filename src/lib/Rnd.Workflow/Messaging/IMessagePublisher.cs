using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rnd.Workflow.Messaging
{
    public interface IMessagePublisher
    {
        Task Publish<T>(T message, CancellationToken cancellationToken = default)
            where T : class;

        //Doesnt work on rabbitmq
        //Task PublishScheduled<T>(T message, DateTime targetTime, CancellationToken cancellationToken = default) where T : class;
    }
}