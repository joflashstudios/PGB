using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGBLib.IO;
using System.IO;

namespace PGBLib.Core
{
    class UpdateBackupTask : BackupTask
    {
        public override void Run()
        {
            OperationManager manager = new OperationManager();
            manager.Start();
            DirectoryScanner sourceScanner = new DirectoryScanner("source");
            sourceScanner.Blacklist = new HashSet<string>();
            sourceScanner.ScannerErrored += (UnauthorizedAccessException e) => { System.Diagnostics.Debugger.Log(1, "nonsense", e.Message); };
            foreach(FileInfo sourceFile in sourceScanner)
            {
                FileInfo destinationFile = new FileInfo("translateSourceToDest");
                if (!destinationFile.Exists || destinationFile.LastWriteTime < sourceFile.LastWriteTime)
                {
                    manager.AddOperation(new CopyOperation(sourceFile.FullName, destinationFile.FullName));
                }
            }
        }
    }
}
