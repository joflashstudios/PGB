using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PGBLib.Core
{
    class IncrementalBackupAbstractionLayer : IDisposable
    {
        private IncrementalBackup Backup;

        public IncrementalBackupAbstractionLayer(IncrementalBackup backup)
        {
            Backup = backup;
        }

        public FileInfo GetFile(string file, DateTime throughDate)
        {
            return GetFile(file, Backup.Steps.Last(n => n.Date <= throughDate));
        }

        public FileInfo GetFile(string file, IncrementalBackupStep throughStep)
        {
            FileInfo fileInfo = null;
            foreach (IncrementalBackupStep step in Backup.Steps)
            {
                if (step.FileAdded(file))
                {
                    fileInfo = new FileInfo(step.Path + file);
                }
                else if (step.FileDeleted(file))
                {
                    fileInfo = null;
                }

                if (step == throughStep)
                    break;
            }
            return fileInfo;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
