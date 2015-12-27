using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGBLib.IO;

namespace PGBLib.Core
{
    public class BackupProgressEventArgs : EventArgs
    {
        OperationProgressDetails OperationDetails { get; }
        long BytesPending { get; }
        long BytesComplete { get; }
        int OperationsPending { get; }
        int OperationsComplete { get; }
        int PercentComplete
        {
            get
            {
                if (OperationsPending == 1)
                    return (OperationsComplete * 100) / (OperationsPending + OperationsComplete);
                else
                    return (int)
                        (((OperationsComplete * 100) / (OperationsPending + OperationsComplete)) +
                        ((BytesComplete * 100) / (BytesPending + BytesComplete)))
                        / 2;
            }
        }

        public BackupProgressEventArgs(OperationProgressDetails details, long bytesPending, long bytesComplete, int operationsPending, int operationsComplete)
        {
            this.OperationDetails = details;
            this.BytesPending = bytesPending;
            this.BytesComplete = bytesComplete;
            this.OperationsPending = operationsPending;
            this.OperationsComplete = operationsComplete;
        }
    }

    public delegate void BackupProgressEventHandler(object sender, BackupProgressEventArgs e);
}
