using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsApp.Tests
{
    public class BaseTestContext
    {
        private SynchronousTaskScheduler SynchronousTaskScheduler { get; set; } = new SynchronousTaskScheduler();

        /// <summary>
        /// This method ensures Action being executed is run in single-threaded environment.
        /// This is important to test methods that are using multiple threads, so that we can be sure that
        /// after this method call completes, all threads in the Action have been completed.
        ///
        /// This only works if the thread being spawned by the Action uses Task.Factory.StartNew
        /// (rather than Task.Run).
        /// </summary>
        /// <param name="action">Action to execute.</param>
        protected Task RunInSingleThread(Action action)
        {
            return Task.Factory.StartNew(() =>
            {
                action();
            }, CancellationToken.None, TaskCreationOptions.None, SynchronousTaskScheduler);
        }
    }
}
