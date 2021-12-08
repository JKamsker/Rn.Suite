using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MongoDB.Driver;

using Rnd.Workflow.Abstractions;
using Rnd.Workflow.Domain;
using Rnd.MongoDb;

namespace Rnd.Workflow.MongoDb
{
    public class MongoWorkflowDao<TEntity> : IWorkflowJobDao<TEntity>
    {
        private readonly IMongoCollection<WorkflowItem<TEntity>> _mongoCollection;
        private readonly ILogger<MongoWorkflowDao<TEntity>> _logger;

        public MongoWorkflowDao
        (
            IMongoCollection<WorkflowItem<TEntity>> mongoCollection,
            ILogger<MongoWorkflowDao<TEntity>> logger
        )
        {
            _mongoCollection = mongoCollection;
            _logger = logger;
        }

        public async Task<string> EnqueueTaskAsync(TEntity entity, string initialStatus)
        {
            var workflowItem = WorkflowItem.Create(entity, initialStatus);
            await _mongoCollection.InsertOneAsync(workflowItem);
            return workflowItem.Id;
        }

        public async Task EnqueueManyTasksAsync(params WorkflowItem<TEntity>[] items)
        {
            await _mongoCollection.InsertManyAsync(items);
        }

        public async Task ResetFailedTasks(string status)
        {
            var updateDefinition = Builders<WorkflowItem<TEntity>>.Update
                    .Set(x => x.MetaData.IsActive, false)
                    .Set(x => x.MetaData.HasFailed, false)
                ;

            Expression<Func<WorkflowItem<TEntity>, bool>> filter =
                string.IsNullOrEmpty(status)
                    ? x => true
                    : x => x.MetaData.Status == status;

            await _mongoCollection.UpdateManyAsync(filter, updateDefinition);
        }

        public async Task ResetStatusAsync(string status = null)
        {
            var updateDefinition = Builders<WorkflowItem<TEntity>>.Update
                .Set(x => x.MetaData.IsActive, false)
                //.Set(x => x.MetaData.LastUsed, DateTimeOffset.UtcNow)
                //.Set(x => x.MetaData.LastUsed, DateTimeOffset.UtcNow.ToUnixTimeSeconds())

            ;

            var filter = Builders<WorkflowItem<TEntity>>.Filter.Where(x => !x.MetaData.HasFailed && x.MetaData.IsActive);

            if (!string.IsNullOrEmpty(status))
            {
                filter = filter.Where(x => x.MetaData.Status == status);
            }

            await _mongoCollection.UpdateManyAsync(filter, updateDefinition);
        }

        public async Task<WorkflowItem<TEntity>> GetJobAsync(string pickupStatus)
        {
            // var sw = Stopwatch.StartNew();
            try
            {
                var updateDefinition = Builders<WorkflowItem<TEntity>>.Update
                    .Set(x => x.MetaData.IsActive, true)
                    //.Set(x => x.MetaData.LastUsed, DateTimeOffset.UtcNow)
                    .Set(x => x.MetaData.LastUsed, DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                    ;

                var currentDateTime = DateTimeOffset.UtcNow;

                var filterDefinition = Builders<WorkflowItem<TEntity>>.Filter.Where(x =>
                    x.MetaData.Status == pickupStatus
                    && !x.MetaData.HasFailed
                    && !x.MetaData.IsActive

                    && ((x.MetaData.ScheduleTime != null && x.MetaData.ScheduleTime <= currentDateTime) ||
                        (x.MetaData.ScheduleTime == null)));

                var sortDefinition = Builders<WorkflowItem<TEntity>>.Sort.Ascending(x => x.MetaData.LastUsed);

                var options = new FindOneAndUpdateOptions<WorkflowItem<TEntity>, WorkflowItem<TEntity>>
                {
                    Sort = sortDefinition,
                };

                return await _mongoCollection.FindOneAndUpdateAsync
                (
                    filter: filterDefinition,
                    update: updateDefinition,
                    options: options
                );
            }
            finally
            {
                //_logger.LogInformation($"Query perf: {sw.Elapsed.TotalMilliseconds} ms");
            }
        }

        public Task UpdateJobFailed(WorkflowItem<TEntity> job, CancellationToken cancellationToken)
            => UpdateJobFailed(job, string.Empty, cancellationToken);

        public async Task UpdateJobFailed(WorkflowItem<TEntity> job, string failureMessage, CancellationToken cancellationToken)
        {
            var updateDefinition = Builders<WorkflowItem<TEntity>>.Update
                .Set(x => x.MetaData.HasFailed, true)
                .Set(x => x.MetaData.IsActive, false)
                //.Set(x => x.MetaData.LastUsed, DateTimeOffset.UtcNow)
                .Set(x => x.MetaData.LastUsed, DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                .Set(x => x.MetaData.FailureMessage, failureMessage)
            ;

            var updateResult = await _mongoCollection.UpdateOneAsync
            (
                x => x.Id == job.Id,
                updateDefinition,
                cancellationToken: cancellationToken
            );

            //TODO: Do smth with it maybe
        }

        public async Task UpdateJobFinished(WorkflowItem<TEntity> job, string nextStatus, CancellationToken cancellationToken)
        {
            var updateDefinition = Builders<WorkflowItem<TEntity>>.Update
                .Set(x => x.MetaData.IsActive, false)
                .Set(x => x.MetaData.Status, nextStatus)
                //.Set(x => x.MetaData.LastUsed, DateTimeOffset.UtcNow)
                .Set(x => x.MetaData.LastUsed, DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                .Set(x => x.Data, job.Data)
                ;

            var updateResult = await _mongoCollection.UpdateOneAsync
            (
                x => x.Id == job.Id,
                updateDefinition,
                cancellationToken: cancellationToken
            );

            //TODO: Do smth with it maybe
        }

        public async Task DelayJobAsync(WorkflowItem<TEntity> job, DateTimeOffset scheduleTime, CancellationToken cancellationToken)
        {
            var updateDefinition = Builders<WorkflowItem<TEntity>>.Update
                    .Set(x => x.MetaData.IsActive, false)
                    .Set(x => x.MetaData.ScheduleTime, scheduleTime)
                    .Set(x => x.Data, job.Data)
                    //.Set(x => x.MetaData.LastUsed, DateTimeOffset.UtcNow)
                    .Set(x => x.MetaData.LastUsed, DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                    ;

            var updateResult = await _mongoCollection.UpdateOneAsync
            (
                x => x.Id == job.Id,
                updateDefinition,
                cancellationToken: cancellationToken
            );

            //TODO: Do smth with it maybe
        }

        public async Task SetLastUseDate(string id, DateTimeOffset dateTimeOffset, CancellationToken cancellationToken = default)
        {
            var updateDefinition = Builders<WorkflowItem<TEntity>>.Update
                .Set(x => x.MetaData.LastUsed, dateTimeOffset.ToUnixTimeSeconds());

            var updateResult = await _mongoCollection.UpdateOneAsync
            (
                x => x.Id == id,
                updateDefinition,
                cancellationToken: cancellationToken
            );
        }
    }
}