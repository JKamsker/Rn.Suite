using System.Threading.Tasks;

namespace Rnd.Workflow.Messaging
{
    public class NullMessageConsumer<TMessage> : IMessageConsumer<TMessage> 
        where TMessage : class
    {
        public Task ConsumeAsync(TMessage message)
        {
            return Task.CompletedTask;
        }
    }
}