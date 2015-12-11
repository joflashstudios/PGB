using System.IO;

namespace PGBLib.IO
{
    public class DeleteOperation : IOOperation
    {
        /// <summary>
        /// Whether to delete the folder after deleting the last file out of it on a delete operation
        /// </summary>
        public bool DeleteEmptyFolder { get; set; }

        public override long EffectiveFileSize
        {
            get
            {
                return 0;
            }
        }

        public override void DoOperation()
        {
            System.IO.File.Delete(FileName);
            string directory = Path.GetDirectoryName(FileName);
            if (DeleteEmptyFolder && Directory.GetFileSystemEntries(directory).Length == 0)
            {
                Directory.Delete(directory);
            }
        }
    }
}
