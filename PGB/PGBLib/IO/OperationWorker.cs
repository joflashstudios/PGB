using System;
using System.Collections.Generic;
using System.Threading;
using PGBLib.IO.Win32;

namespace PGBLib.IO
{
    internal class OperationWorker : IDisposable
    {
        public OperationWorker()
        {
            workerThread = new Thread(new ThreadStart(DoWork));
            OperationQueue = new Queue<IOOperation>();
        }

        public event OperationProgressHandler ProgressMade;

        private Queue<IOOperation> OperationQueue { get; }

        private bool copyTerminateFlag = false;

        /// <summary>
        /// Gets the current number of bytes in pending move or copy operations.
        /// This figure does NOT include the bytes of incomplete in-progress operations, which is available via the ProgressMade event.
        /// </summary>
        public long CopyBytesPending { get { return copyBytesPending; } }
        private long copyBytesPending = 0;

        /// <summary>
        /// Gets the current number of bytes in completed move or copy operations.
        /// This figure does NOT include progress on in-progress operations, which is available via the ProgressMade event.
        /// Includes errored copies.
        /// </summary>
        public long CopyBytesCompleted { get { return copyBytesCompleted; } }
        private long copyBytesCompleted = 0;

        /// <summary>
        /// Gets the current number of pending file operations
        /// </summary>
        public int OperationsPending { get { return OperationQueue.Count; } }

        /// <summary>
        /// Gets the current number of operations that have completed processing.
        /// </summary>
        public int OperationsProcessed { get { return operationsProcessed; } }
        private int operationsProcessed = 0;

        public OperationState State { get { return state; } }
        private OperationState state;

        private Thread workerThread;
        public Thread WorkerThread { get { return workerThread; } }

        /// <summary>
        /// Start the OperationWorker, or resume from a paused state.
        /// </summary>
        public void Start()
        {
            if (State != OperationState.Paused)
            {
                workerThread.Start();
            }
            state = OperationState.Running;
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
            
            //Polymorphism FTW.
            copyBytesPending += op.EffectiveFileSize;
        }

        private void DoWork()
        {
            while (State != OperationState.Terminated)
            {
                if (State != OperationState.Paused)
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
                        Thread.Sleep(0);
                }
                else //We are paused. Yield our current time slice.
                    Thread.Sleep(0);
            }
        }

        private void ProcessOperation(IOOperation operation)
        {
            try
            {
                //Copy operations get some special callbacks and tracking
                CopyOperation copyOp = operation as CopyOperation;
                if (copyOp != null)
                {
                    copyBytesPending -= operation.EffectiveFileSize;
                    CopyProgressCallback copyCall = new CopyProgressCallback((total, transferred, sourceFile, destinationFile) => {
                        ProgressMade(this, new OperationProgressDetails(operation, transferred, total));
                        return CopyProgressResult.PROGRESS_CONTINUE;
                    });
                    copyOp.DoOperation(copyCall, ref copyTerminateFlag);
                }
                else
                {
                    operation.DoOperation();
                }

                //Notify the caller that the operation has completed
                ProgressMade(this, new OperationProgressDetails(operation, true));
            }
            catch (Exception e) //We're catching everything (*gasp!*) so we can bubble it up without blowing up stacks of threads.
            {
                ProgressMade(this, new OperationProgressDetails(operation, e));
            }
            finally
            {
                if (operation is CopyOperation)
                {   //Even if it fails, it's no longer pending. We don't want disappearing bytes.
                    copyBytesCompleted += operation.EffectiveFileSize;
                    operationsProcessed += 1;
                }
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