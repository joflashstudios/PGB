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
        
        private OperationManager _manager;
        private bool _scanComplete;

        public override void Run()
        {
            _scanComplete = false;

            _manager = new OperationManager();
            Thread scanThread = new Thread(ScanDirectories);
            scanThread.Name = "Scanner thread for update backup " + Source;
            _manager.ProgressMade += Manager_ProgressMade;
            _manager.Start();
            scanThread.Start();
        }

        private void Manager_ProgressMade(object sender, OperationProgressDetails progress)
        {
            OperationManager manager = (OperationManager) sender;
            BackupProgressEventArgs args = new BackupProgressEventArgs(progress, manager.BytesPending, manager.BytesProcessed, manager.OperationsPending, manager.OperationsProcessed);
            OnBackupProgress(args);

            if (_scanComplete && manager.OperationsPending == 0)
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

            _scanComplete = true;
        }

        private void ScanDestination()
        {
            DirectoryScanner scanTron = new DirectoryScanner(Destination);
            foreach (FileInfo file in scanTron)
            {
                FileInfo matchingSourceFile = new FileInfo(file.TranslatePath(Destination, Source));
                if (!matchingSourceFile.Exists)
                {
                    DeleteOperation delete = new DeleteOperation(file.FullName);
                    delete.DeleteEmptyFolder = true;
                    _manager.AddOperation(delete);
                }
            }
        }

        private void ScanSource()
        {
            DirectoryScanner scanTron = new DirectoryScanner(Source);
            foreach (FileInfo file in scanTron)
            {
                FileInfo destination = new FileInfo(file.TranslatePath(Source, Destination));

                if (destination.Exists && (destination.LastWriteTime >= file.LastWriteTime))
                    continue;

                CopyOperation copy = new CopyOperation(file.FullName, destination.FullName);

                copy.Overwrite = true;
                copy.CreateFolder = true;

                _manager.AddOperation(copy);
            }
        }
    }
}
