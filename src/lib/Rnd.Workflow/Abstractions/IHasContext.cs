namespace Rnd.Workflow.Abstractions
{
    public interface IHasContext<TContext>
    {
        TContext Context { get; set; }
    }
}