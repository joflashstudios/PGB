using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGBLib.IO;
using System.IO;
using System.Threading;

namespace PGBLib.Core
{
    [Serializable]
    public class UpdateBackup : Backup
    {
        public bool RemoveDeletedFiles { get; set; }
        
        private OperationManager manager;
        private Thread scanThread;

        private bool scanComplete;

        public override void Run()
        {
            manager = new OperationManager();
            scanThread = new Thread(new ThreadStart(ScanDirectories));
            scanThread.Name = "Scanner thread for update backup " + Source;
            manager.ProgressMade += Manager_ProgressMade;
            manager.Start();
            scanThread.Start();
        }

        private void Manager_ProgressMade(object sender, OperationProgressDetails progress)
        {
            BackupProgressEventArgs args = new BackupProgressEventArgs(progress, manager.BytesPending, manager.BytesProcessed, manager.OperationsPending, manager.OperationsProcessed);
            OnBackupProgress(args);

            if (scanComplete && manager.OperationsPending == 0)
            {
                manager.Dispose();
                OnComplete();
            }
        }

        private void ScanDirectories()
        {
            ScanSource();
            if (RemoveDeletedFiles)
                ScanDestination();

            scanComplete = true;
        }

        private void ScanDestination()
        {
            DirectoryScanner scanTron = new DirectoryScanner(Destination);
            foreach (FileInfo file in scanTron)
            {
                FileInfo matchingSourceFile = new FileInfo(Helpers.TransformPath(Destination, Source, file.FullName));
                if (!matchingSourceFile.Exists)
                {
                    DeleteOperation delete = new DeleteOperation(file.FullName);
                    delete.DeleteEmptyFolder = true;
                    manager.AddOperation(delete);
                }
            }
        }

        private void ScanSource()
        {
            DirectoryScanner scanTron = new DirectoryScanner(Source);
            foreach (FileInfo file in scanTron)
            {
                FileInfo destination = new FileInfo(Helpers.TransformPath(Source, Destination, file.FullName));

                if (destination.Exists && (destination.LastWriteTime >= file.LastWriteTime))
                    continue;

                CopyOperation copy = new CopyOperation(file.FullName, destination.FullName);

                copy.Overwrite = true;
                copy.CreateFolder = true;

                manager.AddOperation(copy);
            }
        }
    }
}
