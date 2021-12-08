using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Rnd.Workflow.Abstractions;

namespace Rnd.Workflow.Configuration
{
    public class WorkflowConfigurator : IWorkflowConfigurator
    {
        public IServiceCollection ServiceCollection { get; set; }

        public WorkflowConfigurator(IServiceCollection collection)
        {
            ServiceCollection = collection;
        }
    }
}
