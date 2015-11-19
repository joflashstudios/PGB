using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PGBLib.IO
{
    internal class OperationWorker : IDisposable
    {
        public OperationWorker()
        {
            _workerThread = new Thread(new ThreadStart(DoWork));
        }

        public Queue<IOOperation> OperationQueue { get; set; }

        private bool _terminate = false;
        private Thread _workerThread;
        public Thread WorkerThread { get { return _workerThread; } }

        public void Start()
        {
            _workerThread.Start();
        }

        private void DoWork()
        {
            while (!_terminate)
            {
                IOOperation currentOperation = null;
                lock (OperationQueue)
                {
                    if (OperationQueue.Count > 0)
                        currentOperation = OperationQueue.Dequeue();
                }

                if (currentOperation != null)
                {
                    
                }
                else //We have nothing to do. Yield our current time slice.
                {
                    Thread.Sleep(0);
                }
            }
        }

        public void Dispose()
        {
            _terminate = true;
        }

        public void Terminate()
        {
            Dispose();
        }
        //public List<IOOperation> CompletedOperations { get; set; } //What's the best way to get this information to the OperationManager? Maybe actually use a WorkerProgress event.... yeah.
    }
}
