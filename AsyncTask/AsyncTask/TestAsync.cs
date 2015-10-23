using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AsyncTask
{
    public class TestAsync
    {
                
        public void testAsyncMultipleWithSemaphore()
        {
            var throttler = new SemaphoreSlim(30);

            var taskNos = new List<int>();
            int nTasks = 5;
            for (int i = 0; i < nTasks; i++)
            {
                taskNos.Add(i);
            }

            var tasks = new List<Task<int>>();

            Console.WriteLine("firing tasks");
            var cts = new CancellationTokenSource();
            
            taskNos.ForEach((i) =>
            {                
                throttler.Wait(cts.Token);
                tasks.Add(Task.Run(() =>
                {
                    var result = doSomething(i);
                    throttler.Release();
                    return result;
                }));
            });

            Console.WriteLine("All tasks are fired");

            try
            {
                Task.WaitAll(tasks.ToArray(), 10000, cts.Token);
            }
            catch (AggregateException aException) 
            {
                Console.WriteLine("Aggregate Exceptions Caught : " + aException.InnerExceptions.Count);
            }
            catch (Exception ae)
            {
                Console.WriteLine("Caught an exception during Task.WaitAll : " + ae.GetType() + "; Message : " + ae.Message);
            }

            Console.WriteLine("Main thread is complete, press any key to exit...");
        }

        private int doSomething(int threadNo)
        {            
            Thread.Sleep(1000 * threadNo);
            if (threadNo  == 3) {
                throw new Exception("Exception started inside task number : " + threadNo);
            }
            Console.WriteLine("Task no : " + threadNo + " completed successfully");
            return threadNo;
        }
    }
}
