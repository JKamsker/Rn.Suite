using MassTransit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

using Rnd.Logging;
using Rnd.Workflow;
using Rnd.Workflow.Abstractions;
using Rnd.Workflow.Core;
using Rnd.Workflow.Domain;
using Rnd.Workflow.Extensions;
using Rnd.Workflow.Messaging;
using Rnd.Workflow.Messaging.MassTransit;
using Rnd.Workflow.MongoDb;
using Rnd.Workflow.Step;

using System.Drawing;
using Rnd.Workflow.MongoDb.Extensions;

using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace RndPg_5
{
    internal class Program
    {
        private static void Main(string[] args) =>
            Host
                .CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    BsonClassMap.RegisterClassMap<WorkflowItem<MyEntity>>(cm =>
                    {
                        cm.AutoMap();
                        cm.MapIdMember(c => c.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
                    });

                    var mongoClient = new MongoClient(new MongoClientSettings
                    {
                        Server = new MongoServerAddress("localhost", 27017)
                    });

                    var db = mongoClient.GetDatabase("WorkflowTest");

                    services
                        .AddSingleton(db)
                        .ConfigureWorkflowDb<MyEntity>(db)
                        ;

                    services.ConfigureWorkflowEngine(wf => wf
                        .UseMassTransit(x => x.UsingRabbitMq(DefaultConfiguration.DefaultLocalRabbitMqConfiguration))
                        .AddWorkflowStep<SampleWorkflowStep>("Enter", "1", 24)
                        .AddWorkflowStep<SampleWorkflowStep01>("1", "2", 24)
                        .AddWorkflowStep<SampleWorkflowStepEnd>("2", "Leave", 24)
                    );

                    services.AddHostedService<MainWorker>();
                })
                .ConfigureLogging
                (
                    x => x.ClearProviders()
                        .AddColorConsoleLogger(LogLevel.Warning, Color.DarkMagenta)
                        .AddColorConsoleLogger(LogLevel.Error, Color.Red)
                        .AddColorConsoleLogger(LogLevel.Trace, Color.Gray)
                        .AddColorConsoleLogger(LogLevel.Debug, Color.Gray)
                        .AddColorConsoleLogger(LogLevel.Information, Color.Yellow)
                        .AddColorConsoleLogger(LogLevel.Critical, Color.Red)
                )
                .Build()
                .Run();

        private static void UseNoMessageBus(IServiceCollection services)
        {
            services.AddSingleton<IMessagePublisher, NullMessagePublisher>();
            services.AddSingleton<IMessageDistributor<WorkflowItemStatusChangedEvent>, NullMessageDistributorr<WorkflowItemStatusChangedEvent>>();
        }

        private static void UseMasstransitMessageBus(IServiceCollection services, bool inMemory = true)
        {
            services.AddSingleton<IMessagePublisher, MassTransitMessagePublisher>();
            services.AddSingleton<IMessageDistributor<WorkflowItemStatusChangedEvent>>(x => x.GetRequiredService<MessageRedistributor<WorkflowItemStatusChangedEvent>>());

            services.AddSingleton<MessageRedistributor<WorkflowItemStatusChangedEvent>>();
            services.AddSingleton<IMessageConsumer<WorkflowItemStatusChangedEvent>>(x => x.GetRequiredService<MessageRedistributor<WorkflowItemStatusChangedEvent>>());

            services.AddMassTransit(x =>
            {
                x.AddConsumer<MassTransitMessageConsumer<WorkflowItemStatusChangedEvent>>();

                if (inMemory)
                {
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.TransportConcurrencyLimit = 100;
                        cfg.ConfigureEndpoints(context);
                    });
                    return;
                }

                // 5672: Queue port
                // 15672: Management port
                // docker run -d --hostname my-rabbit --name some-rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management
                x.UsingRabbitMq((context, cfg) =>
                {
                    //cfg.Host("rabbitmq://127.0.0.1:15672");
                    cfg.Durable = false;
                    cfg.Exclusive = false;
                    cfg.ConfigureEndpoints(context);
                });
            }).AddMassTransitHostedService(true);
        }
    }

    
}