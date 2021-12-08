using System;
using Rnd.Workflow.Core;

namespace Rnd.Workflow.Step
{
    public class WorkflowConfigurationItem
    {
        public WorkflowConfigurationItem(Type workflowStepType, string sourceStep, string targetStep, int instanceCount = 1)
        {
            WorkflowStepType = workflowStepType;
            SourceStep = sourceStep;
            TargetStep = targetStep;
            InstanceCount = instanceCount;
        }

        public Type WorkflowStepType { get; }
        public string SourceStep { get; }
        public string TargetStep { get; }
        public int InstanceCount { get; }

        public WorkflowEngineStatusDescriptor AsDescriptor() => new(SourceStep, TargetStep);
    }
}