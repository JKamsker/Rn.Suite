using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Rnd.Workflow.Abstractions;
using Rnd.Workflow.Domain;
using Rnd.Workflow.Exceptions;
using Rnd.Workflow.Messaging;
using Rnd.Workflow.Step;

namespace Rnd.Workflow.Core
{
    public class WorkflowStepRunner<TEntity>
    {
        private TaskCompletionSource<bool> _stopSource;

        private readonly WorkflowEngineStatusDescriptor _descriptor;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IWorkflowJobDao<TEntity> _workflowJobDao;
        private readonly IWorkflowStepFactory<TEntity> _workflowStepFactory;
        private readonly ILogger<WorkflowStepRunner<TEntity>> _logger;

        private volatile bool _idling = false;

        public bool Running { get; private protected set; }

        public bool Idle => _idling;

        public string StepId { get; }

        public WorkflowStepRunner
        (
            IWorkflowJobDao<TEntity> workflowJobDao,
            IWorkflowStepFactory<TEntity> workflowStepFactory,
            ILogger<WorkflowStepRunner<TEntity>> logger,
            WorkflowEngineStatusDescriptor statusDescriptor,
            IMessagePublisher messagePublisher
        )
        {
            _descriptor = statusDescriptor;
            _messagePublisher = messagePublisher;
            _workflowJobDao = workflowJobDao;
            _workflowStepFactory = workflowStepFactory;
            _logger = logger;
            StepId = statusDescriptor.Pickup;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (Running)
            {
                return Task.CompletedTask;
            }

            Running = true;

            _ = Task.Run(() => WorkflowLoop(cancellationToken), cancellationToken);
            return Task.CompletedTask;
        }

        public virtual async Task<bool> StopAsync()
        {
            if (!Running)
            {
                return false;
            }

            if (_stopSource != null)
            {
                return false;
            }

            _stopSource = new TaskCompletionSource<bool>();
            return await _stopSource.Task;
        }

        public virtual async Task ResetStatusAsync()
        {
            await _workflowJobDao.ResetStatusAsync(_descriptor.Pickup);
        }

        private protected virtual async Task WorkflowLoop(CancellationToken cancellationToken = default)
        {
            if (_descriptor == null)
            {
                throw new ArgumentException("Invalid descriptor");
            }

            while (Running && !cancellationToken.IsCancellationRequested)
            {
                var job = await _workflowJobDao.GetJobAsync(_descriptor.Pickup);
                if (job == null)
                {
                    _idling = true;

                    //_logger.LogInformation($"Delaying {_descriptor.Pickup}");
                    await Delay(cancellationToken);
                    continue;
                }

                _idling = false;
                var wfStepInstance = _workflowStepFactory.MakeWorkflowStep(new WorkflowStepContext(job.Id, job.MetaData, _descriptor));
                var result = await ExecuteJob(job, wfStepInstance, cancellationToken);
                if (result is OkStepResult okStep)
                {
                    var targetStep = !string.IsNullOrEmpty(okStep.TargetStep) ? okStep.TargetStep : _descriptor.Next;

                    var message = new WorkflowItemStatusChangedEvent(job.Id, job.MetaData.Status, targetStep);
                    await _workflowJobDao.UpdateJobFinished(job, targetStep, cancellationToken);
                    await _messagePublisher.Publish(message, cancellationToken);
                    _logger.LogInformation($"[{job.Id}] Job succeeded");
                }
                else if (result is DelayStepResult delayStep)
                {
                    //var message = new WorkflowItemStatusChangedEvent(job.Id, job.MetaData.Status, _descriptor.Next);
                    var scheduledTime = DateTimeOffset.UtcNow + delayStep.Delay;

                    await _workflowJobDao.DelayJobAsync(job, scheduledTime, cancellationToken);
                    //await _messagePublisher.PublishScheduled(message, scheduledTime.UtcDateTime, cancellationToken);
                    _logger.LogInformation($"[{job.Id}] Job delayed");
                }
                else if (result is FailureStepResult failureStepResult)
                {
                    // Todo: Messages?
                    await _workflowJobDao.UpdateJobFailed(job, failureStepResult.Message, cancellationToken);
                    _logger.LogWarning($"[{job.Id}] Job failed ({failureStepResult.Message})");
                }
                else
                {
                    await _workflowJobDao.UpdateJobFailed(job, cancellationToken);
                    _logger.LogWarning($"[{job.Id}] Job failed because of an unknown reason");
                }
            }

            _stopSource?.SetResult(true);
            _stopSource = null;
        }

        private async Task<IWorkflowStepResult> ExecuteJob(WorkflowItem<TEntity> job, IWorkflowStep<TEntity> wfStepInstance, CancellationToken cancellationToken)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    return await wfStepInstance.OnDoWork(job.Data, cancellationToken);
                }
                catch (WorkflowJobFailedException e)
                {
                    return new FailureStepResult(e.Message);
                }
                catch (Exception e)
                {
                    // Todo: Messages?
                    _logger.LogCritical(e, $"[{job.Id}] An exception occured '{e.Message}'");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
            return WorkflowStepResult.Failure;
        }

        private TaskCompletionSource<bool> _currentDelay;

        private async Task Delay(CancellationToken cancellationToken)
        {
            var delay = _currentDelay = new();

            _ = Task.Delay(TimeSpan.FromSeconds(2), cancellationToken).ContinueWith(x =>
           {
               delay?.TrySetResult(true);
           });

            await delay.Task;
        }

        public void TryCancelDelay()
        {
            _currentDelay?.TrySetResult(true);
            _currentDelay = null;
        }
    }
}