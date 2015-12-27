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
        public OperationProgressDetails OperationDetails { get; }
        public long BytesPending { get; }
        public long BytesComplete { get; }
        public int OperationsPending { get; }
        public int OperationsComplete { get; }
        public int PercentComplete
        {
            get
            {
                if (OperationsPending == 1)
                    return (int)((BytesComplete * 100) / (BytesPending + BytesComplete));
                else
                    return (int)
                        (((OperationsComplete * 100) / (OperationsPending + OperationsComplete)) +
                        (((BytesComplete + OperationDetails.BytesProcessed) * 100) / (BytesPending + BytesComplete + OperationDetails.BytesTotal)))
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
