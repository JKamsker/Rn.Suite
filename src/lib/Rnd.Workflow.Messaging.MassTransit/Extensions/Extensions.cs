using System;
using System.Collections.Generic;
using System.Text;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rnd.Workflow.Abstractions;
using Rnd.Workflow.Domain;

namespace Rnd.Workflow.Messaging.MassTransit
{
    public static class DependencyInjectionExtensions
    {
        public static IWorkflowConfigurator UseMassTransit(this IWorkflowConfigurator workflowConfigurator, Action<IServiceCollectionBusConfigurator> configure = null)
        {
            var services = workflowConfigurator.ServiceCollection;
            services.TryAddSingleton<IMessagePublisher, MassTransitMessagePublisher>();
            services.TryAddSingleton<IMessageDistributor<WorkflowItemStatusChangedEvent>>(x => x.GetRequiredService<MessageRedistributor<WorkflowItemStatusChangedEvent>>());

            services.TryAddSingleton<MessageRedistributor<WorkflowItemStatusChangedEvent>>();
            services.TryAddSingleton<IMessageConsumer<WorkflowItemStatusChangedEvent>>(x => x.GetRequiredService<MessageRedistributor<WorkflowItemStatusChangedEvent>>());


            services.AddMassTransit(x =>
            {
                x.AddConsumer<MassTransitMessageConsumer<WorkflowItemStatusChangedEvent>>();
                x.AddDelayedMessageScheduler();
                configure?.Invoke(x);
                
                
                //if (inMemory)
                //{
                //    x.UsingInMemory((context, cfg) =>
                //    {
                //        cfg.TransportConcurrencyLimit = 100;
                //        cfg.ConfigureEndpoints(context);
                //    });
                //    return;
                //}

                //// 5672: Queue port
                //// 15672: Management port
                //// docker run -d --hostname my-rabbit --name some-rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management
                //x.UsingRabbitMq((context, cfg) =>
                //{
                //    //cfg.Host("rabbitmq://127.0.0.1:15672");
                //    cfg.Durable = false;
                //    cfg.Exclusive = false;
                //    cfg.ConfigureEndpoints(context);
                //});
            }).AddMassTransitHostedService(true);
            
            return workflowConfigurator;
        }
    }
}