using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rnd.Lib.Extensions;
using Rnd.Workflow.Abstractions;
using Rnd.Workflow.Domain;
using Rnd.Workflow.Messaging;
using Rnd.Workflow.Step;

namespace Rnd.Workflow.Core
{
    public class WorkflowContainer<TEntity> : IMessageConsumer<WorkflowItemStatusChangedEvent>
    {
        private readonly IEnumerable<WorkflowConfigurationItem> _configurationItems;
        private readonly IMessageDistributor<WorkflowItemStatusChangedEvent> _messageDistributor;
        private readonly ILogger<WorkflowContainer<TEntity>> _logger;
        private readonly IServiceProvider _serviceProvider;

        private bool _started = false;
        private List<WorkflowStepRunner<TEntity>> _runners;

        private SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);

        public bool Idle => _runners.All(x => x.Idle);

        public WorkflowContainer
        (
            IServiceProvider serviceProvider,
            IEnumerable<WorkflowConfigurationItem> configurationItems,
            IMessageDistributor<WorkflowItemStatusChangedEvent> messageDistributor,
            ILogger<WorkflowContainer<TEntity>> logger
        )
        {
            _configurationItems = configurationItems;
            _messageDistributor = messageDistributor;

            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_started)
            {
                return;
            }

            Debug.Assert(_runners == null);

            using var _ = await _syncLock.LockAsync(cancellationToken);
            if (_started)
            {
                return;
            }

            _started = true;

            var runnerFactory = ActivatorUtilities.CreateFactory(typeof(WorkflowStepRunner<TEntity>), new[]
            {
                typeof(WorkflowEngineStatusDescriptor),
                typeof(IWorkflowStepFactory<TEntity>)
            });

            var runners = new List<WorkflowStepRunner<TEntity>>();
            _logger.LogInformation($"Creating Runners");
            foreach (var configurationItem in _configurationItems)
            {
                runners.AddRange(CreateRunners(runnerFactory, configurationItem));
            }

            _logger.LogInformation($"Waiting for runners to come up");
            await Task.WhenAll(runners.Select(x => x.StartAsync(cancellationToken)));

            _runners = runners;

            await _messageDistributor.SubscribeAsync(this);
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (!_started)
            {
                return;
            }

            using var _ = await _syncLock.LockAsync(cancellationToken);
            if (!_started)
            {
                return;
            }

            await _messageDistributor.UnSubscribeAsync(this);

            await Task.WhenAll(_runners.Select(x => x.StopAsync()));
            _runners = null;
        }

        private IReadOnlyList<WorkflowStepRunner<TEntity>> CreateRunners(ObjectFactory runnerFactory, WorkflowConfigurationItem configurationItem)
        {
            var descriptor = configurationItem.AsDescriptor();
            var stepFactory = new FuncWorkflowStepFactory<TEntity>
            (
                metaData =>
                {
                    var service = _serviceProvider.GetService(configurationItem.WorkflowStepType);
                    if (service is IHasContext<WorkflowStepContext> contextContainer)
                    {
                        contextContainer.Context = metaData;
                    }

                    return (IWorkflowStep<TEntity>)service;
                }
            );

            return Enumerable.Range(0, configurationItem.InstanceCount)
                .Select(x => runnerFactory(_serviceProvider, new object[] { descriptor, stepFactory }))
                .Cast<WorkflowStepRunner<TEntity>>()
                .ToList()
                .AsReadOnly();
        }

        public Task ConsumeAsync(WorkflowItemStatusChangedEvent message)
        {
            foreach (var runner in _runners.Where(x => x.StepId == message.NewStatus))
            {
                if (!runner.Running)
                {
                    continue;
                }

                if (!runner.Idle)
                {
                    continue;
                }

                runner.TryCancelDelay();
                
            }

            return Task.CompletedTask;
        }
    }
}