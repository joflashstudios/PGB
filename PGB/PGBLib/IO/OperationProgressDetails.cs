using System;

namespace PGBLib.IO
{
    class OperationProgressDetails : EventArgs
    {   
        public OperationProgressType ProgressType { get; }
        public IOOperation Operation { get; }
        public bool Completed { get; }
        public long BytesTransferred { get; }
        public long BytesTotal { get; }
        public Exception Error { get; }

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

        public OperationProgressDetails(IOOperation operation, long bytesTransferred, long bytesTotal)
        {
            Operation = operation;
            BytesTransferred = bytesTransferred;
            BytesTotal = bytesTotal;

            ProgressType = OperationProgressType.InProgress;
        }
    }

    enum OperationProgressType
    {
        Generic,
        Completed,
        Errored,
        InProgress
    }
}
