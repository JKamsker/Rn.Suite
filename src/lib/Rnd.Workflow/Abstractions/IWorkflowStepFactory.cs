using Rnd.Workflow.Core;
using Rnd.Workflow.Step;

namespace Rnd.Workflow.Abstractions
{
    public interface IWorkflowStepFactory<in TEntity>
    {
        IWorkflowStep<TEntity> MakeWorkflowStep(WorkflowStepContext metaData);
    }
}