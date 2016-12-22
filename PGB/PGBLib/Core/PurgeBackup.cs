using System;
using System.IO;
using System.Threading;
using PGBLib.IO;

namespace PGBLib.Core
{
    [Serializable]
    public class PurgeBackup : Backup
    {
        /// <summary>
        /// The duration after which a file should be considered stale, and moved to the archive
        /// </summary>
        public TimeSpan StaleTime { get; set; }

        /// <summary>
        /// The additional duration after which a file should be considered expired, and removed from the archive
        /// </summary>
        public TimeSpan ExpiredTime { get; set; }

        private OperationManager Manager { get; set; }
        private bool _sourceScanComplete;
        private bool _destionationScanComplete;

        public override void Run()
        {
            _sourceScanComplete = _destionationScanComplete = false;

            if (string.IsNullOrEmpty(Source) || string.IsNullOrEmpty(Destination))
                throw new InvalidOperationException("Source and Destination must both be defined.");

            if (StaleTime == TimeSpan.Zero || ExpiredTime == TimeSpan.Zero)
                throw new InvalidOperationException("StaleTime and ExpiredTime must be non-zero.");

            Manager = new OperationManager();

            Thread sourceScanThread = new Thread(ScanSource);
            sourceScanThread.Name = "Source scanner thread for purge backup " + Source;

            Thread destinationScanThread = new Thread(ScanDestination);
            destinationScanThread.Name = "Destination scanner thread for purge backup " + Source;

            Manager.ProgressMade += Manager_ProgressMade;

            Manager.Start();
            sourceScanThread.Start();
            destinationScanThread.Start();
        }

        private void ScanSource()
        {
            DirectoryScanner scanTron = new DirectoryScanner(Source);

            foreach (FileInfo file in scanTron)
            {
                if (file.IsUntouched(StaleTime))
                {
                    MoveOperation operation = new MoveOperation(file.FullName, file.TranslatePath(Source, Destination));
                    operation.DeleteEmptyFolder = true;
                    Manager.AddOperation(operation);
                }
            }
            _sourceScanComplete = true;
        }

        private void ScanDestination()
        {
            DirectoryScanner scanTron = new DirectoryScanner(Destination);

            foreach (FileInfo file in scanTron)
            {
                if (file.IsUntouched(ExpiredTime))
                {
                    DeleteOperation operation = new DeleteOperation(file.FullName);
                    operation.DeleteEmptyFolder = true;
                    Manager.AddOperation(operation);
                }
            }
            _destionationScanComplete = true;
        }

        private void Manager_ProgressMade(object sender, OperationProgressDetails progress)
        {
            OperationManager manager = (OperationManager)sender;
            BackupProgressEventArgs args = new BackupProgressEventArgs(progress, manager.BytesPending, manager.BytesProcessed, manager.OperationsPending, manager.OperationsProcessed);
            OnBackupProgress(args);

            if (_sourceScanComplete && _destionationScanComplete && manager.OperationsPending == 0)
            {
                manager.Dispose();
                OnComplete();
            }
        }
    }
}
