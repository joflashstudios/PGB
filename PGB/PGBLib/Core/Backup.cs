using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGBLib.Core
{
    class Backup
    {
        public Backup(string name)
        {
            Name = name;
        }

        public void Run()
        {
            Tasks.ForEach(n => n.Run());
        }

        public List<BackupTask> Tasks { get; set; }
        public string Name { get; }
    }
}
