using System;
using Rnd.Workflow.Abstractions;
using Rnd.Workflow.Core;

namespace Rnd.Workflow.Step
{
    public class DefaultWorkflowStepFactory<TEntity> : IWorkflowStepFactory<TEntity>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly WorkflowConfiguration _map;

        public DefaultWorkflowStepFactory
        (
            IServiceProvider serviceProvider,
            WorkflowConfiguration map
        )
        {
            _serviceProvider = serviceProvider;
            _map = map;
        }

        public IWorkflowStep<TEntity> MakeWorkflowStep(WorkflowStepContext metaData)
        {
            var requiredType = _map.FindByDescriptor(metaData.Descriptor);
            var result = (IWorkflowStep<TEntity>)_serviceProvider.GetService(requiredType);
            if (result is IHasContext<WorkflowStepContext> contextContainer)
            {
                contextContainer.Context = metaData;
            }

            return result;
        }
    }
}