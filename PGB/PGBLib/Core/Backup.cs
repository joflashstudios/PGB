using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace PGBLib.Core
{
    [Serializable]
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

        public void SaveToFile(string fileName)
        {
            using (FileStream writer = new FileStream(fileName, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(writer, this);
            }
        }
    }
}
