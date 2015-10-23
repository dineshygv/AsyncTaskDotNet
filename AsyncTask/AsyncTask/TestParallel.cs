using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncTask
{
    public class TestParallel
    {
        public void StartTestParallel() { 
            var taskNos = new List<int>();

            int nTasks = 10;

            for (int i = 0; i < nTasks; i++)
            {
                taskNos.Add(i);
            }

            var cancellationToken = new CancellationTokenSource();

            var parallelOptions = new ParallelOptions { 
                MaxDegreeOfParallelism = 3,
                CancellationToken = cancellationToken.Token
            };

            Action<int, ParallelLoopState> parallelAction = (task, parallelLoopState) => {
                try
                {
                    doSomething(task);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Caught exception inside action : " + ex.Message);
                    cancellationToken.Cancel();
                }
            };

            ParallelLoopResult parallelResult;
            try
            {
                parallelResult = Parallel.ForEach(taskNos, parallelOptions, parallelAction);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.GetType() + " : " + ex.Message);
            }
                        
        }

        private int doSomething(int threadNo)
        {
            Console.WriteLine("Started Task No : " + threadNo);

            Thread.Sleep(1000 * threadNo);

            if (threadNo  == 3) {
                throw new Exception("Exception started inside task number : " + threadNo);
            }

            Console.WriteLine("Task no : " + threadNo + " completed successfully");
            return threadNo;
        }
    }


    
}
