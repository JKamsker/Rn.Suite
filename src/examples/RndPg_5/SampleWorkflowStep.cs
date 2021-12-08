using System;
using Microsoft.Extensions.Logging;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;
using Rnd.Workflow.Abstractions;
using Rnd.Workflow.Core;
using Rnd.Workflow.Domain;
using Rnd.Workflow.Step;

namespace Rnd.Workflow
{
    public class MyEntity
    {
        public int Number { get; set; }
    }

    public class SampleWorkflowStep : WorkflowStepBase<MyEntity>, IHasContext<WorkflowStepContext>
    {
        private readonly ILogger<SampleWorkflowStep> _logger;

        public WorkflowStepContext Context { get; set; }

        public SampleWorkflowStep
        (
            ILogger<SampleWorkflowStep> logger
        )
        {
            _logger = logger;
        }

        public override async Task<IWorkflowStepResult> OnDoWork(MyEntity job, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Working on {Context.Id}");
            if (job.Number < 5)
            {
                _logger.LogInformation("Number below 5; Delaying...");
                job.Number++;
                return new DelayStepResult(TimeSpan.FromSeconds(1));
            }
            
            await Task.Delay(100, cancellationToken);
            job.Number *= 10;
            await Task.Delay(100, cancellationToken);
            _logger.LogInformation($"Worked on {Context.Id}");

            return base.Ok();
        }
    }

    public class SampleWorkflowStep01 : WorkflowStepBase<MyEntity>
    {
        private readonly ILogger<SampleWorkflowStep> _logger;

        public SampleWorkflowStep01
        (
            ILogger<SampleWorkflowStep> logger
        )
        {
            _logger = logger;
        }

        public override async Task<IWorkflowStepResult> OnDoWork(MyEntity job, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Step2 start");

            await Task.Delay(100, cancellationToken);
            job.Number *= 2;
            await Task.Delay(100, cancellationToken);
            _logger.LogInformation($"Step2 End");

            return base.Ok();
        }
    }

    public class SampleWorkflowStepEnd : WorkflowStepBase<MyEntity>, IHasContext<WorkflowStepContext>
    {
        private readonly ILogger<SampleWorkflowStep> _logger;
        public WorkflowStepContext Context { get; set; }

        public SampleWorkflowStepEnd
        (
            ILogger<SampleWorkflowStep> logger
        )
        {
            _logger = logger;
        }

        public override async Task<IWorkflowStepResult> OnDoWork(MyEntity job, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Result: {job.Number}");
            return base.Ok();
        }
    }
}