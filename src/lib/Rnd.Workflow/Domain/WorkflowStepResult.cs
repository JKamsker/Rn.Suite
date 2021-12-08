using System;
using System.Collections.Generic;
using System.Text;
using Rnd.Workflow.Exceptions;

namespace Rnd.Workflow.Domain
{
    public static class WorkflowStepResult
    {
        public static IWorkflowStepResult Ok => new OkStepResult();
        public static IWorkflowStepResult Failure => new FailureStepResult();
    }

    public interface IWorkflowStepResult
    {
    }

    public class OkStepResult : IWorkflowStepResult
    {
        public string TargetStep { get; }

        public OkStepResult()
        {
            
        }

        public OkStepResult(string targetStep)
        {
            TargetStep = targetStep;
        }
    }

    public class FailureStepResult : IWorkflowStepResult
    {
        public string Message { get; set; }

        public FailureStepResult()
        {
        }

        public FailureStepResult(string message)
        {
            Message = message;
        }
        
        public static implicit operator Exception(FailureStepResult failureStepResult) => new WorkflowJobFailedException(failureStepResult.Message);
        public static implicit operator FailureStepResult(Exception failureStepResult) => new(failureStepResult.Message);

    }

    public class DelayStepResult : IWorkflowStepResult
    {
        public TimeSpan Delay { get; }

        public DelayStepResult(TimeSpan delay)
        {
            Delay = delay;
        }
    }
}