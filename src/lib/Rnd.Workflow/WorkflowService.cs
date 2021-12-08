using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rnd.Workflow.Abstractions;
using Rnd.Workflow.Domain;
using Rnd.Workflow.Messaging;

namespace Rnd.Workflow
{
    public class WorkflowService<TJob>
    {
        private readonly IWorkflowJobDao<TJob> _dao;
        private readonly IMessagePublisher _bus;

        public WorkflowService(IWorkflowJobDao<TJob> dao, IMessagePublisher bus)
        {
            _dao = dao;
            _bus = bus;
        }

        public async Task<string> EnqueueTaskAsync(TJob job, string status)
        {
            var jobId = await _dao.EnqueueTaskAsync(job, status);
            await _bus.Publish(new WorkflowItemStatusChangedEvent(jobId, string.Empty, status));
            return jobId;
        }

        public async Task<string[]> EnqueueManyAsync(IEnumerable<TJob> jobs, string status)
        {
            var wfItems = jobs.Select(x => WorkflowItem.Create(x, status)).ToArray();
            if (!wfItems.Any())
            {
                return Array.Empty<string>();
            }
            
            await _dao.EnqueueManyTasksAsync(wfItems);

            var result = wfItems.Select(x => x.Id).ToArray();

            foreach (var item in result)
            {
                await _bus.Publish(new WorkflowItemStatusChangedEvent(item, string.Empty, status));
            }

            return result;
        }
    }
}