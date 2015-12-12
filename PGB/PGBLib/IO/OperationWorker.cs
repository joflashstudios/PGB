using System;
using System.Collections.Generic;
using System.Threading;
using PGBLib.IO.Win32;

namespace PGBLib.IO
{
    internal class OperationWorker : IDisposable
    {
        public event OperationProgressHandler ProgressMade
        {
            add
            {
                lock (eventLock)
                {
                    progressHandler += value;
                }
            }
            remove
            {
                lock (eventLock)
                {
                    progressHandler -= value;
                }
            }
        }

        /// <summary>
        /// Gets the current number of pending file operations
        /// </summary>
        public int OperationsPending { get { return operationQueue.Count; } }
        /// <summary>
        /// Gets the current number of bytes in pending move or copy operations.
        /// </summary>
        public long BytesPending { get { return bytesPending; } }
        /// <summary>
        /// Gets the current number of operations that have completed processing.
        /// </summary>
        public int OperationsProcessed { get { return operationsProcessed; } }
        /// <summary>
        /// Gets the current number of bytes in completed move or copy operations. Includes errored operations.
        /// </summary>
        public long BytesProcessed { get { return bytesProcessed; } }

        public OperationState State { get { return state; } }

        public Thread[] WorkerThreads { get { return workerThreads; } }

        public string Name { get; }


        private OperationProgressHandler progressHandler;

        private long bytesPending = 0;
        private int operationsProcessed = 0;
        private long bytesProcessed = 0;

        private OperationState state;

        private Thread[] workerThreads;

        private readonly object eventLock = new object();
        private readonly object statLock = new object();

        private Queue<IOOperation> operationQueue { get; }

        private bool copyTerminateFlag = false;

        public OperationWorker(int workerCount, string name)
        {
            Name = name;
            operationQueue = new Queue<IOOperation>();
            workerThreads = new Thread[workerCount];
            for (int i = 0; i < workerCount; i++)
            {
                workerThreads[i] = new Thread(new ThreadStart(DoWork)) { Name = name + " worker thread #" + (i + 1) };
            }
        }

        /// <summary>
        /// Start the OperationWorker, or resume from a paused state.
        /// </summary>
        public void Start()
        {
            if (State != OperationState.Paused)
            {
                foreach (Thread t in workerThreads)
                {
                    t.Start();
                }
            }
            state = OperationState.Running;
        }

        public void Terminate()
        {
            Dispose();
        }

        public void Pause()
        {
            state = OperationState.Paused;
        }

        public void Dispose()
        {
            copyTerminateFlag = true;
            state = OperationState.Terminated;
        }

        public void EnqueueOperation(IOOperation op)
        {
            operationQueue.Enqueue(op);
            
            lock(statLock)
            {
                //Polymorphism FTW.
                bytesPending += op.EffectiveFileSize;
            }
        }


        private void DoWork()
        {
            while (State != OperationState.Terminated)
            {
                if (State != OperationState.Paused)
                {
                    IOOperation currentOperation = null;
                    lock (operationQueue)
                    {
                        if (operationQueue.Count > 0)
                            currentOperation = operationQueue.Dequeue();
                    }

                    if (currentOperation != null)
                        ProcessOperation(currentOperation);
                    else //We have nothing to do. Yield.
                        Thread.Sleep(100);
                }
                else //We are paused. Yield.
                    Thread.Sleep(100);
            }
        }

        private void ProcessOperation(IOOperation operation)
        {
            lock (statLock)
                bytesPending -= operation.EffectiveFileSize;

            try
            {
                //Copy operations get some special callbacks and tracking
                CopyOperation copyOp = operation as CopyOperation;
                if (copyOp != null)
                {                    
                    CopyProgressCallback copyCall = new CopyProgressCallback((total, transferred, sourceFile, destinationFile) => {
                        OnProgress(new OperationProgressDetails(operation, transferred, total));
                        return CopyProgressResult.PROGRESS_CONTINUE;
                    });
                    copyOp.DoOperation(copyCall, ref copyTerminateFlag);
                }
                else
                {
                    operation.DoOperation();
                }
            }
            catch (Exception e) //We're catching everything (*gasp!*) so we can bubble it up without blowing up stacks of threads.
            {
                OnProgress(new OperationProgressDetails(operation, e));
            }
            finally
            {
                lock(statLock)
                { 
                    bytesProcessed += operation.EffectiveFileSize;
                    operationsProcessed += 1;
                }

                //Notify the caller that the operation has completed
                OnProgress(new OperationProgressDetails(operation, true));
            }
        }

        private void OnProgress(OperationProgressDetails details)
        {
            OperationProgressHandler handler;
            lock (eventLock)
            {
                handler = progressHandler;
            }
            if (handler != null)
            {
                handler(this, details);
            }
        }
    }
}