using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGBLib.Core
{
    [Serializable]
    class IncrementalBackup : Backup
    {
        public SortedSet<IncrementalBackupStep> Steps { get { return steps; } }

        private SortedSet<IncrementalBackupStep> steps = new SortedSet<IncrementalBackupStep>(new IncrementalBackupStepComparer());

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }

    class IncrementalBackupStepComparer : IComparer<IncrementalBackupStep>
    {
        public int Compare(IncrementalBackupStep x, IncrementalBackupStep y)
        {
            if (x.Date < y.Date)
                return -1;
            else if (x.Date > y.Date)
                return 1;
            else
                return 0;
        }
    }
}
