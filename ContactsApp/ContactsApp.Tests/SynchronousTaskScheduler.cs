using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsApp.Tests
{
    /// <summary>
    /// Reference:
    /// - https://stackoverflow.com/questions/17844182/wait-until-all-task-finish-in-unit-test.
    /// - https://helpercode.com/2014/11/23/unit-testing-concurrent-code-using-custom-taskscheduler.
    /// </summary>
    public class SynchronousTaskScheduler : TaskScheduler
    {
        public override int MaximumConcurrencyLevel => 1;

        protected override void QueueTask(Task task)
        {
            TryExecuteTask(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool wasPreviouslyQueued)
        {
            return TryExecuteTask(task);
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return Enumerable.Empty<Task>();
        }
    }
}
