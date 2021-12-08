using System;
using System.Collections.Generic;
using System.Text;

using MassTransit;
using MassTransit.RabbitMqTransport;

namespace Rnd.Workflow.Messaging.MassTransit
{
    public static class DefaultConfiguration
    {
        public static readonly Action<IBusRegistrationContext, IInMemoryBusFactoryConfigurator> DefaultInMemoryConfiguration = (context, cfg) =>
        {
            cfg.TransportConcurrencyLimit = 100;
            cfg.UseDelayedMessageScheduler();
            cfg.ConfigureEndpoints(context);
        };

        public static readonly Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> DefaultLocalRabbitMqConfiguration = (context, cfg) =>
            {
                //cfg.Host("rabbitmq://127.0.0.1:15672");
                cfg.Durable = false;
                cfg.Exclusive = false;
                cfg.UseDelayedMessageScheduler();
                cfg.ConfigureEndpoints(context);
                
            };
    }
}