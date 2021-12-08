using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using Rnd.Workflow.Abstractions;
using Rnd.Workflow.Domain;

namespace Rnd.Workflow.MongoDb.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection ConfigureWorkflowDb<TEntity>(this IServiceCollection serviceCollection, IMongoDatabase database, string collectionName = null)
        {
            collectionName = !string.IsNullOrEmpty(collectionName) ? collectionName : $"Workflow_{typeof(TEntity).Name}";

            var wfCollection = database.GetCollection<WorkflowItem<TEntity>>(collectionName);
            // wfCollection.UpdateMany(x => true, Builders<WorkflowItem<TEntity>>.Update.Set(x => x.MetaData.LastUsedNew, DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
            //wfCollection.UpdateMany(x => true, Builders<WorkflowItem<TEntity>>.Update.Rename("MetaData.LastUsedNew", "MetaData.LastUsed"));
            wfCollection.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<WorkflowItem<TEntity>>(Builders<WorkflowItem<TEntity>>.IndexKeys.Hashed(wfItem => wfItem.MetaData.Status)),
                //new CreateIndexModel<WorkflowItem<TEntity>>(Builders<WorkflowItem<TEntity>>.IndexKeys.Ascending(wfItem => wfItem.MetaData.LastUsed)),
                new CreateIndexModel<WorkflowItem<TEntity>>(Builders<WorkflowItem<TEntity>>.IndexKeys.Ascending(wfItem => wfItem.MetaData.LastUsed)),
            });

            serviceCollection
                .AddSingleton(wfCollection)
                .AddSingleton<IWorkflowJobDao<TEntity>, MongoWorkflowDao<TEntity>>()
                ;

            return serviceCollection;
        }
    }
}