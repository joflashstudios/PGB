using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PGBLib.IO.Win32;
using System.IO;

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
        /// Gets the current number of bytes in pending or in-progress move or copy operations.
        /// </summary>
        public long CopyBytesPending { get { return copyBytesPending; } }
        private long copyBytesPending = 0;

        /// <summary>
        /// Gets the current number of pending file operations
        /// </summary>
        public int OperationsPending { get { return OperationQueue.Count; } }

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

            if (op is CopyOperation)
            {
                if (File.Exists(op.FileName))
                {
                    copyBytesPending += new FileInfo(op.FileName).Length;
                }
            }
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
                CopyOperation copyOp = operation as CopyOperation;
                if (copyOp != null)
                {
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
                ProgressMade(this, new OperationProgressDetails(operation, true));
            }
            catch (Exception e) //We're catching everything (*gasp!*) so we can bubble it up without blowing up stacks of threads.
            {
                ProgressMade(this, new OperationProgressDetails(operation, e));
            }
            finally
            {
                if (operation is CopyOperation)
                {
                    copyBytesPending -= operation.EffectiveFileSize;
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