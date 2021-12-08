using System;
using Rnd.Workflow.Core;
using Rnd.Workflow.Domain;

namespace Rnd.Workflow.Step
{
    public class WorkflowStepContext
    {
        public string Id { get; }
        public WorkflowEngineStatusDescriptor Descriptor { get; }
        public DateTimeOffset LastUsed { get; }

        public string Status { get; }

        public bool IsActive { get; }
        public bool HasFailed { get; }

        public WorkflowStepContext
        (
            string id, 
            WorkflowItemMetaData metaData,
            WorkflowEngineStatusDescriptor descriptor
        )
        {
            Id = id;
            Descriptor = descriptor;
            LastUsed = DateTimeOffset.FromUnixTimeSeconds(metaData.LastUsed);
            Status = metaData.Status;
            IsActive = metaData.IsActive;
            HasFailed = metaData.HasFailed;
        }
    }
}