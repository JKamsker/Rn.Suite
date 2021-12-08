using System.Threading.Tasks;

namespace Rnd.Workflow.Messaging
{
    public class NullMessageDistributorr<TMessage> : IMessageDistributor<TMessage>
        where TMessage : class
    {
        public Task SubscribeAsync(IMessageConsumer<TMessage> consumer)
        {
            return Task.CompletedTask;
        }

        public Task UnSubscribeAsync(IMessageConsumer<TMessage> consumer)
        {
            return Task.CompletedTask;
        }
    }
}