using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Rnd.Workflow.Abstractions;
using Rnd.Workflow.Configuration;
using Rnd.Workflow.Core;
using Rnd.Workflow.Domain;
using Rnd.Workflow.Messaging;
using Rnd.Workflow.Step;

namespace Rnd.Workflow.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddWorkflowStep<TStep>
        (
            this IServiceCollection services,
            string sourceStep,
            string targetStep,
            int instanceCount = 1
        ) where TStep : class
        {
            var configServiceDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(WorkflowConfiguration));
            if (configServiceDescriptor?.ImplementationInstance is not WorkflowConfiguration config)
            {
                services.RemoveAll(typeof(WorkflowConfiguration));

                config = new WorkflowConfiguration();
                services.Add(new ServiceDescriptor(typeof(WorkflowConfiguration), config));
                services.AddSingleton<IEnumerable<WorkflowConfigurationItem>>(x => x.GetRequiredService<WorkflowConfiguration>());
            }

            config.Add(typeof(TStep), sourceStep, targetStep, instanceCount);
            services.AddTransient<TStep>();

            return services;
        }

        public static IWorkflowConfigurator AddWorkflowStep<TStep>
        (
            this IWorkflowConfigurator configurator,
            string sourceStep,
            string targetStep,
            int instanceCount = 1
        ) where TStep : class
        {
            configurator.ServiceCollection.AddWorkflowStep<TStep>(sourceStep, targetStep, instanceCount);
            return configurator;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">Cheapo configuration callback, just for the esthetics</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureWorkflowEngine(this IServiceCollection services, Action<IWorkflowConfigurator> configuration = null)
        {
            services.TryAddSingleton(typeof(WorkflowService<>));
            services.TryAddTransient(typeof(IWorkflowStepFactory<>), typeof(DefaultWorkflowStepFactory<>));
            services.TryAddTransient(typeof(WorkflowContainer<>));

            var configurator = new WorkflowConfigurator(services);


            configuration?.Invoke(configurator);

            //If configuration doesnt contain messaging, we have to use the default way
            services.TryAddSingleton<IMessagePublisher, NullMessagePublisher>();
            services.TryAddSingleton<IMessageDistributor<WorkflowItemStatusChangedEvent>, NullMessageDistributorr<WorkflowItemStatusChangedEvent>>();

            return services;
        }
    }
}