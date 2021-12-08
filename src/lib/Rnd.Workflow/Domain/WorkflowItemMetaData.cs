using System;

namespace Rnd.Workflow.Domain
{
    public class WorkflowItemMetaData
    {
        //public DateTimeOffset LastUsed { get; set; }
        public long LastUsed { get; set; }

        public string Status { get; set; }

        public bool IsActive { get; set; }
        public bool HasFailed { get; set; }

        public string FailureMessage { get; set; }

        public DateTimeOffset? ScheduleTime { get; set; }

        public DateTimeOffset GetLastUsed() => DateTimeOffset.FromUnixTimeSeconds(LastUsed);
        public void SetLastUsed(DateTimeOffset dateTimeOffset) => LastUsed = dateTimeOffset.ToUnixTimeSeconds();
    }
}