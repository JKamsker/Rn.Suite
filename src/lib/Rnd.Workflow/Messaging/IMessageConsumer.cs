using System.Threading.Tasks;

namespace Rnd.Workflow.Messaging
{
    public interface IMessageConsumer<in TMessage> where TMessage : class
    {
        Task ConsumeAsync(TMessage message);
    }
}