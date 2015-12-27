using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGBLib.Core
{
    public abstract class Backup
    {
        public abstract void Run();

        public string Source { get; set; }

        public string Destination { get; set; }

        public event BackupProgressEventHandler BackupProgress;

        public event Action Complete;

        protected void OnBackupProgress(BackupProgressEventArgs e)
        {
            if (BackupProgress != null)
                BackupProgress(this, e);
        }

        protected void OnComplete()
        {
            if (Complete != null)
                Complete();
        }
    }
}
