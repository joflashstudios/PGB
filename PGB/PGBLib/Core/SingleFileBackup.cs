using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGBLib.IO;

namespace PGBLib.Core
{
    [Serializable]
    public class SingleFileBackup : Backup
    {
        public bool DeleteSource { get; set; }
        public bool Overwrite { get; set; }

        public SingleFileBackup() { }

        public SingleFileBackup(string source, string destination)
        {
            Source = source;
            Destination = destination;
        }

        //Normally the list-building logic would be in its own method on a separate thread. But this is a bit of a special case.
        public override void Run()
        {
            if (string.IsNullOrEmpty(Source) || string.IsNullOrEmpty(Destination))
                throw new InvalidOperationException("Source and Destination must both be defined.");
            CopyOperation operation = BuildOperation();

            OperationWorker worker = new OperationWorker(1, "Single file backup: " + Source);
            worker.ProgressMade += Worker_ProgressMade;
            worker.EnqueueOperation(operation);
            worker.Start();
        }

        private CopyOperation BuildOperation()
        {
            CopyOperation operation;
            if (DeleteSource)
                operation = new MoveOperation();
            else
                operation = new CopyOperation();

            operation.CreateFolder = true;
            operation.FileName = Source;
            operation.TransferDestination = Destination;
            operation.Overwrite = Overwrite;
            return operation;
        }

        private void Worker_ProgressMade(object sender, OperationProgressDetails progress)
        {
            if (progress.Completed)
            {
                ((OperationWorker)sender).Dispose();
                OnComplete();
            }
            else
            {
                OnBackupProgress(new BackupProgressEventArgs(progress, progress.BytesPending, progress.BytesProcessed, 1, 0));
            }
        }
    }
}
