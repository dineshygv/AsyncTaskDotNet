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
        /**
         *  Call this method to test a single async task 
         * 
         */
        public void testAsyncSingle() {
            Task<int> task = runAsync(1);
            Console.WriteLine("task is running");

            //wait till task is complete
            task.Wait();

            //get the task result
            var result = task.Result;
            Console.WriteLine("result is " + result);
            Console.ReadKey();

        }

        /**
         *  Call this method o try running multiple async tasks with some timeout and some throwing exceptions 
         * 
         */
        public void testAsyncMultipleWithTimeOutAndException() {
            var taskNos = new List<int>();
            int nTasks = 5;
            for (int i = 0; i < nTasks; i++) {
                taskNos.Add(i);
            }

            var tasks = new List<Task>();
            taskNos.ForEach((i) => {
                tasks.Add(Task.Run(() => doSomething(i)));
            });

            Console.WriteLine("all tasks are fired");

            try
            {
                //wait for all tasks to complete
                //7000 is 7 seconds timeout
                Task.WaitAll(tasks.ToArray(), 7000);
            }catch(AggregateException ae){

                //handle each of the exceptions inside the aggregate exception
                ae.Handle((x) => {
                    Console.WriteLine("exception : " + x.Message);
                    return true;
                });
            }

            Console.WriteLine("all tasks are done");
            Console.ReadKey();
        }

        public void testAsyncMultipleWithSemaphore()
        {
            var throttler = new SemaphoreSlim(3);

            var taskNos = new List<int>();
            int nTasks = 10;
            for (int i = 0; i < nTasks; i++)
            {
                taskNos.Add(i);
            }

            var tasks = new List<Task>();

            Console.WriteLine("firing tasks");

            taskNos.ForEach((i) =>
            {
                // wait till a semaphore handle is available before firing the task
                throttler.Wait();
                tasks.Add(Task.Run(() => {
                    try
                    {
                        doSomething(i);
                    }
                    finally {

                        // release the semaphore handle once task is done
                        throttler.Release();
                    }     
                }));
            });

            Console.WriteLine("all tasks are fired");

            try
            {
                Task.WaitAll(tasks.ToArray(), 7000);
            }
            catch (AggregateException ae)
            {
                ae.Handle((x) =>
                {
                    Console.WriteLine("exception : " + x.Message);
                    return true;
                });
            }

            Console.WriteLine("all tasks are done");
            Console.ReadKey();
        }

        private int doSomething(int threadNo)
        {
            if (threadNo %2 == 1) {
                throw new Exception("Too many tasks heating up my brain no " + threadNo);
            }
            Console.WriteLine("task " + threadNo + " started");
            Thread.Sleep(1000);
            //Thread.Sleep(threadNo * 1000);
            Console.WriteLine("task " + threadNo + " finished");
            return threadNo;
        }

        private async Task<int> runAsync(int threadNo)
        {
            await Task.Run(() => Thread.Sleep(threadNo * 1000));
            return threadNo;
        }
    }
}
