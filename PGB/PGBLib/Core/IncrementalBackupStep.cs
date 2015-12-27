using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace PGBLib.Core
{
    [Serializable]
    public class IncrementalBackupStep
    {
        public IncrementalBackupStep(string path, bool isMaster)
        {
            Path = path;
            Date = DateTime.Now;
            IsMaster = isMaster;
        }

        public string Path { get; }
        public DateTime Date { get; }

        //Masters only store deleted files.
        public bool IsMaster { get; }

        public void DumpData()
        {
            data = null;
        }

        public bool FileDeleted(string file)
        {
            LoadData();
            return data.FileDeleted(file);
        }

        public bool FileAdded(string file)
        {
            if (IsMaster)
                return File.Exists(Path + file);

            LoadData();
            return data.FileAdded(file);
        }

        private void LoadData()
        {
            if (data == null)
                data = IncrementalBackupStepData.LoadFromFile(Path + "data.pgbd");
        }

        private IncrementalBackupStepData data;

        private class IncrementalBackupStepData
        {
            public IncrementalBackupStepData()
            {
                Deletions = new HashSet<string>();
                Additions = new HashSet<string>();
            }

            HashSet<string> Deletions { get; }
            HashSet<string> Additions { get; }

            public bool FileDeleted(string file)
            {
                return Deletions.Contains(file);
            }

            public bool FileAdded(string file)
            {
                return Additions.Contains(file);
            }

            public static IncrementalBackupStepData LoadFromFile(string file)
            {
                using (FileStream stream = new FileStream(file, FileMode.Open))
                {
                    BinaryFormatter cornFlakes = new BinaryFormatter();
                    return (IncrementalBackupStepData)cornFlakes.Deserialize(stream);
                }
            }
        }
    }
}
