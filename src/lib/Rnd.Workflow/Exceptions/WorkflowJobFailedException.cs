using System;
using System.Collections.Generic;
using System.Text;

namespace Rnd.Workflow.Exceptions
{
    [Serializable]
    public class WorkflowJobFailedException : Exception
    {
        public WorkflowJobFailedException(string message) 
            : base(message)
        {
            
        }
    }
}
