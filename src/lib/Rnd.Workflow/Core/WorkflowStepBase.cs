using System;
using System.Threading;
using System.Threading.Tasks;

using Rnd.Workflow.Abstractions;
using Rnd.Workflow.Domain;
using Rnd.Workflow.Step;

namespace Rnd.Workflow.Core
{
    public interface IWorkflowStep<in TEntity>
    {
        Task<IWorkflowStepResult> OnDoWork(TEntity postJob, CancellationToken cancellationToken = default);
    }

    public abstract class WorkflowStepBase<TEntity> : IWorkflowStep<TEntity>, IHasContext<WorkflowStepContext>
    {
        public WorkflowStepContext Context { get; set; }

        public abstract Task<IWorkflowStepResult> OnDoWork(TEntity job, CancellationToken cancellationToken = default);

        protected OkStepResult Ok(string targetStep = "") => new OkStepResult();

        protected FailureStepResult Fail(string message = null) => new FailureStepResult(message);
        protected DelayStepResult Delay(TimeSpan timeSpan) => new DelayStepResult(timeSpan);
        
        
    }
}