using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGBLib.IO
{
    class OperationProgressDetails
    {        
        IOOperation Operation { get; }
        bool Completed { get; }
        long BytesTransferred { get; }
        long BytesTotal { get; }
        Exception Error { get; }

        public OperationProgressDetails(IOOperation operation, bool completed)
        {
            Operation = operation;
            Completed = completed;
        }

        public OperationProgressDetails(IOOperation operation, Exception error)
        {
            Operation = operation;
            this.Error = error;
        }

        public OperationProgressDetails(IOOperation operation, long bytesTransferred, long bytesTotal)
        {
            Operation = operation;
            BytesTransferred = bytesTransferred;
            BytesTotal = bytesTotal;
        }
    }
}
