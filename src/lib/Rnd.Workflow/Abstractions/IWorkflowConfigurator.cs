using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Rnd.Workflow.Abstractions
{
    public interface IWorkflowConfigurator
    {
        public IServiceCollection ServiceCollection { get; set; }
    }
}
