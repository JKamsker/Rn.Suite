using System;

namespace Rnd.Workflow.Domain
{
    public class WorkflowItem
    {
        public static WorkflowItem<TEntity> Create<TEntity>(TEntity entity, string initialStatus)
        {
            return new()
            {
                Data = entity,
                MetaData = new WorkflowItemMetaData
                {
                    Status = initialStatus,
                    //LastUsed = DateTimeOffset.UtcNow,
                    LastUsed = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                }
            };
        }
    }

    public class WorkflowItem<TEntity>
    {
        public string Id { get; set; }

        public WorkflowItemMetaData MetaData { get; set; }

        public TEntity Data { get; set; }
    }
}