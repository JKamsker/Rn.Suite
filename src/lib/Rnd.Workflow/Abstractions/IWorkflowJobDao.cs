using System;
using System.Threading;
using System.Threading.Tasks;
using Rnd.Workflow.Domain;

namespace Rnd.Workflow.Abstractions
{
    public interface IWorkflowJobDao<TEntity>
    {
        Task<string> EnqueueTaskAsync(TEntity entity, string initialStatus);

        Task ResetStatusAsync(string status = null);

        Task<WorkflowItem<TEntity>> GetJobAsync(string pickupStatus);

        Task UpdateJobFailed(WorkflowItem<TEntity> job, CancellationToken cancellationToken);

        Task UpdateJobFinished(WorkflowItem<TEntity> job, string nextStatus, CancellationToken cancellationToken);
        Task EnqueueManyTasksAsync(params WorkflowItem<TEntity>[] items);
        Task ResetFailedTasks(string status);
        Task DelayJobAsync(WorkflowItem<TEntity> job, DateTimeOffset scheduleTime, CancellationToken cancellationToken);
        Task UpdateJobFailed(WorkflowItem<TEntity> job, string failureMessage, CancellationToken cancellationToken);
        //Task SetLastUseDate(WorkflowItem<TEntity> job, DateTimeOffset dateTimeOffset, CancellationToken cancellationToken = default);
        Task SetLastUseDate(string id, DateTimeOffset dateTimeOffset, CancellationToken cancellationToken = default);
    }
}