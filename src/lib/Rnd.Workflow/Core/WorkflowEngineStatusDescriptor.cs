namespace Rnd.Workflow.Core
{
    public class WorkflowEngineStatusDescriptor
    {
        public string Pickup { get; }
        public string Next { get; }

        public WorkflowEngineStatusDescriptor(string pickup, string next)
        {
            Pickup = pickup;
            Next = next;
        }
    }
}