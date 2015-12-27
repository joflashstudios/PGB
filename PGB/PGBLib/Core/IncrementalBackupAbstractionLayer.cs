using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGBLib.Core
{
    class IncrementalBackupAbstractionLayer : IDisposable
    {
        private IncrementalBackup Backup;

        public IncrementalBackupAbstractionLayer(IncrementalBackup backup)
        {
            Backup = backup;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
