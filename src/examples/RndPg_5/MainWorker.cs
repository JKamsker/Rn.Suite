using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Rnd.Lib.Extensions;
using Rnd.Workflow;
using Rnd.Workflow.Core;
using Rnd.Workflow.Messaging;

namespace RndPg_5
{
    public class MainWorker : BackgroundService
    {
        private readonly WorkflowContainer<MyEntity> _wfContainer;

        private readonly ILogger<MainWorker> _logger;
        private readonly WorkflowService<MyEntity> _workflowService;

        public MainWorker
        (
            WorkflowContainer<MyEntity> wfContainer,
            ILogger<MainWorker> logger,
            WorkflowService<MyEntity> workflowService
        )

        {
            _wfContainer = wfContainer;
            _logger = logger;
            _workflowService = workflowService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var job = new MyEntity
            {
                Number = 1
            };

            await _wfContainer.StartAsync(stoppingToken);

            // await RunTasks(job);

            await _workflowService.EnqueueTaskAsync(job, "Enter");

            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation($"Enqueueing job");
                //_logger.LogInformation($"Enqueued {jobId}");

                //var jobId = await _dao.EnqueueTaskAsync(job, "Enter");
                //await _bus.Publish(new WorkflowItemStatusChanged(jobId, string.Empty, "Enter"));
                //_logger.LogInformation($"Published message {jobId}");

                _logger.LogInformation($"Waiting, IDLING: ${_wfContainer.Idle}");

                //if (_wfContainer.Idle)
                //{
                //    await RunTasks(job);
                //}

                await Task.Delay(500, stoppingToken);
            }
        }

        private async Task RunTasks(MyEntity job)
        {
            await _workflowService.EnqueueManyAsync(Enumerable.Repeat(job, 1000), "Enter");
        }
    }
}