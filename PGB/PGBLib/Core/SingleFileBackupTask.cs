using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGBLib.IO;

namespace PGBLib.Core
{
    class SingleFileBackupTask : BackupTask
    {
        public bool DeleteSource { get; set; }
        public bool Overwrite { get; set; }

        public SingleFileBackupTask() { }

        public SingleFileBackupTask(string source, string destination)
        {
            Source = source;
            Destination = destination;
        }

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
            OnBackupProgress(new BackupProgressEventArgs(progress, progress.BytesTotal, progress.BytesTransferred, 1, 0));

            if (progress.Completed)
            {
                ((OperationWorker)sender).Dispose();
                OnComplete();
            }
        }
    }
}
