using System;
using System.Collections.Generic;
using System.Threading;
using PGBLib.IO.Win32;

namespace PGBLib.IO
{
    internal class OperationWorker : IDisposable
    {
        public OperationWorker(int workerCount)
        {
            operationQueue = new Queue<IOOperation>();
            for (int i = 0; i < workerCount; i++)
            {
                workerThreads[i] = new Thread(new ThreadStart(DoWork));
            }
        }

        private readonly object eventLock = new object();
        private readonly object statLock = new object();
        private OperationProgressHandler progressHandler;
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

        private Queue<IOOperation> operationQueue { get; }

        private bool copyTerminateFlag = false;

        /// <summary>
        /// Gets the current number of bytes in pending move or copy operations.
        /// This figure does NOT include the bytes of incomplete in-progress operations, which is available via the ProgressMade event.
        /// </summary>
        public long BytesPending { get { return copyBytesPending; } }
        private long copyBytesPending = 0;

        /// <summary>
        /// Gets the current number of bytes in completed move or copy operations.
        /// This figure does NOT include progress on in-progress operations, which is available via the ProgressMade event.
        /// Includes errored copies.
        /// </summary>
        public long BytesProcessed { get { return copyBytesCompleted; } }
        private long copyBytesCompleted = 0;

        /// <summary>
        /// Gets the current number of pending file operations
        /// </summary>
        public int OperationsPending { get { return operationQueue.Count; } }

        /// <summary>
        /// Gets the current number of operations that have completed processing.
        /// </summary>
        public int OperationsProcessed { get { return operationsProcessed; } }
        private int operationsProcessed = 0;

        public OperationState State { get { return state; } }
        private OperationState state;

        private Thread[] workerThreads;
        public Thread[] WorkerThreads { get { return workerThreads; } }

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

        /// <summary>
        /// Enqueues an IOOperation to the Operation Queue.
        /// </summary>
        public void EnqueueOperation(IOOperation op)
        {
            operationQueue.Enqueue(op);
            
            lock(statLock)
            {
                //Polymorphism FTW.
                copyBytesPending += op.EffectiveFileSize;
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
                    else //We have nothing to do. Yield our current time slice.
                        Thread.Sleep(0);
                }
                else //We are paused. Yield our current time slice.
                    Thread.Sleep(0);
            }
        }

        private void ProcessOperation(IOOperation operation)
        {
            lock (statLock)
                copyBytesPending -= operation.EffectiveFileSize;

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

                //Notify the caller that the operation has completed
                OnProgress(new OperationProgressDetails(operation, true));
            }
            catch (Exception e) //We're catching everything (*gasp!*) so we can bubble it up without blowing up stacks of threads.
            {
                OnProgress(new OperationProgressDetails(operation, e));
            }
            finally
            {
                lock(statLock)
                { 
                    copyBytesCompleted += operation.EffectiveFileSize;
                    operationsProcessed += 1;
                }
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

        public void Dispose()
        {
            copyTerminateFlag = true;
            state = OperationState.Terminated;
        }

        public void Terminate()
        {
            Dispose();
        }

        public void Pause()
        {
            state = OperationState.Paused;
        }
    }
}