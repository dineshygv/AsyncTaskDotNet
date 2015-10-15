using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var testAsync = new TestAsync();
            testAsync.testAsyncMultipleWithSemaphore();
        }
                
    }
}
