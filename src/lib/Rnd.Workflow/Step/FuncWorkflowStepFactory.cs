using System;
using Rnd.Workflow.Abstractions;
using Rnd.Workflow.Core;

namespace Rnd.Workflow.Step
{
    public class FuncWorkflowStepFactory<TEntity> : IWorkflowStepFactory<TEntity>
    {
        private readonly Func<WorkflowStepContext, IWorkflowStep<TEntity>> _factory;

        public FuncWorkflowStepFactory(Func<WorkflowStepContext, IWorkflowStep<TEntity>> factory)
        {
            _factory = factory;
        }
        
        public IWorkflowStep<TEntity> MakeWorkflowStep(WorkflowStepContext metaData)
        {
            return _factory(metaData);
        }
    }
}