using System.Threading.Tasks;

namespace Rnd.Workflow.Messaging
{
    public interface IMessageDistributor<out TMessage>
        where TMessage : class
    {
        Task SubscribeAsync(IMessageConsumer<TMessage> consumer);

        Task UnSubscribeAsync(IMessageConsumer<TMessage> consumer);
    }
}