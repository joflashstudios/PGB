using System;

namespace PGBLib.IO
{
    public class OperationProgressDetails : EventArgs
    {   
        public OperationProgressType ProgressType { get; }
        public IOOperation Operation { get; }
        public bool Completed { get; }
        public long BytesProcessed { get; }
        public long BytesPending { get; }
        public Exception Error { get; }
        public int PercentComplete
        {
            get
            {
                if (BytesPending + BytesProcessed == 0)
                    return 0;
                return (int)((BytesProcessed * 100) / (BytesPending + BytesProcessed));
            }
        }

        public OperationProgressDetails(IOOperation operation, bool completed)
        {
            Operation = operation;
            Completed = completed;

            ProgressType = completed ? OperationProgressType.Completed : OperationProgressType.Generic;
        }

        public OperationProgressDetails(IOOperation operation, Exception error)
        {
            Operation = operation;
            this.Error = error;

            ProgressType = error != null ? OperationProgressType.Errored : OperationProgressType.Generic;
        }

        public OperationProgressDetails(IOOperation operation, long bytesProcessed, long bytesPending)
        {
            Operation = operation;
            BytesProcessed = bytesProcessed;
            BytesPending = bytesPending;

            ProgressType = OperationProgressType.InProgress;
        }

        public override string ToString()
        {
            return Operation.ToString() + " is " + ProgressType.ToString();
        }
    }

    public enum OperationProgressType
    {
        Generic,
        Completed,
        Errored,
        InProgress
    }
}
