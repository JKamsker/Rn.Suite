using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rnd.Workflow.Core;

namespace Rnd.Workflow.Step
{
    public class WorkflowConfiguration : IEnumerable, IEnumerable<WorkflowConfigurationItem>
    {
        private readonly List<WorkflowConfigurationItem> _map = new();

        public WorkflowConfiguration()
        {
        }

        public void Add(Type type, string sourceStep, string targetStep, int instanceCount = 1)
        {
            _map.Add(new(type, sourceStep, targetStep, instanceCount));
        }

        IEnumerator<WorkflowConfigurationItem> IEnumerable<WorkflowConfigurationItem>.GetEnumerator()
        {
            return _map.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _map.GetEnumerator();
        }

        public Type FindByDescriptor(WorkflowEngineStatusDescriptor descriptor)
        {
            return _map
                .FirstOrDefault(x => x.SourceStep == descriptor.Pickup && x.TargetStep == descriptor.Next)
                ?.WorkflowStepType;
        }
    }
}