namespace Rnd.Workflow.Domain
{
    public class WorkflowItemStatusChangedEvent
    {
        /// <summary>
        /// Id of the job
        /// </summary>
        public string Id { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }

        public WorkflowItemStatusChangedEvent()
        {
        }

        public WorkflowItemStatusChangedEvent(string id, string oldStatus, string newStatus)
        {
            Id = id;
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }
    }
}