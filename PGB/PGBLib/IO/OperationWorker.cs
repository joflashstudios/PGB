﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PGBLib.IO.Win32;

namespace PGBLib.IO
{
    internal class OperationWorker : IDisposable
    {
        public OperationWorker()
        {
            _workerThread = new Thread(new ThreadStart(DoWork));
            OperationQueue = new Queue<IOOperation>();
        }

        public event OperationProgressHandler ProgressMade;

        public Queue<IOOperation> OperationQueue { get; }

        private bool _terminate = false;
        private Thread _workerThread;
        public Thread WorkerThread { get { return _workerThread; } }

        public void Start()
        {
            _workerThread.Start();
        }

        /// <summary>
        /// Enqueues an IOOperation to the Operation Queue.
        /// This method is thread-safe.
        /// </summary>
        public void EnqueueOperation(IOOperation op)
        {
            lock (OperationQueue)
            {
                OperationQueue.Enqueue(op);
            }
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
                    ProcessOperation(currentOperation);
                }
                else //We have nothing to do. Yield our current time slice.
                {
                    Thread.Sleep(0);
                }
            }
        }

        private void ProcessOperation(IOOperation operation)
        {
            try
            {
                CopyOperation copyOp = operation as CopyOperation;
                if (copyOp != null)
                {
                    CopyProgressCallback copyCall = new CopyProgressCallback((total, transferred, sourceFile, destinationFile) => {
                        ProgressMade(this, new OperationProgressDetails(operation, transferred, total));
                        return CopyProgressResult.PROGRESS_CONTINUE;
                    });
                    copyOp.DoOperation(copyCall, ref _terminate);
                }
                else
                {
                    operation.DoOperation();
                }
                ProgressMade(this, new OperationProgressDetails(operation, true));
            }
            catch (Exception e) //We're catching everything (*gasp!*) so we can bubble it up without blowing up stacks of threads.
            {
                ProgressMade(this, new OperationProgressDetails(operation, e));
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
    }
}