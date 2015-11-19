using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGBLib.IO.Win32;
using System.IO;

namespace PGBLib.IO
{
    class CopyOperation : IOOperation
    {
        /// <summary>
        /// Whether to overwrite or throw an exception if the file already exists
        /// </summary>
        public bool Overwrite { get; set; }

        /// <summary>
        /// Whether to create the destination folder or throw an exception if the folder does not exist
        /// </summary>        
        public bool CreateFolder { get; set; }

        /// <summary>
        /// The destination (if any) to move/copy the file to
        /// </summary>
        public string TransferDestination { get; set; }

        public override void DoOperation()
        {
            DoOperation(null);
        }

        public void DoOperation(CopyProgressCallback callback)
        {
            bool cancel = false;
            FileCopier.Copy(this.File, this.TransferDestination, ref cancel);
        }

        public virtual void DoOperation(CopyProgressCallback callback, ref bool cancel)
        {
            string transferDirectory = Path.GetDirectoryName(TransferDestination);
            if (!Directory.Exists(transferDirectory))
            {
                if (CreateFolder)
                {
                    CloneFoldersUp(Path.GetDirectoryName(File), transferDirectory);
                }
                else
                {
                    throw new DirectoryNotFoundException(string.Format("The directory '{0}' could not be found.", transferDirectory));
                }
            }

            CopyFileFlags flags = 0;
            if (!this.Overwrite)
                flags = flags | CopyFileFlags.COPY_FILE_FAIL_IF_EXISTS;

            FileCopier.Copy(File, TransferDestination, ref cancel, callback, flags);
        }        

        private void CloneFoldersUp(string cloneFrom, string cloneTo)
        {
            if (string.IsNullOrEmpty(cloneFrom))
            {
                if (Directory.Exists(GetParent(cloneTo)))
                {
                    Directory.CreateDirectory(cloneTo);
                }
                else
                {
                    CloneFoldersUp(null, GetParent(cloneTo));
                }
                return;
            }
            
            if (Directory.Exists(GetParent(cloneTo)))
            {
                if (Path.GetDirectoryName(cloneFrom) == Path.GetDirectoryName(cloneTo))
                {
                    DirectoryCloner.CloneDirectory(cloneFrom, cloneTo);
                }
                else
                {
                    Directory.CreateDirectory(cloneTo);
                }
            }
            else
            {
                if (Path.GetDirectoryName(GetParent(cloneFrom)) == Path.GetDirectoryName(GetParent(cloneTo)))
                {
                    CloneFoldersUp(GetParent(cloneFrom), GetParent(cloneTo));
                }
                else
                {
                    CloneFoldersUp(null, GetParent(cloneTo));
                }            
            }
        }

        //NOTE: I'm concerned this isn't robust enough.
        private string GetParent(string path)
        {
            return path.Substring(0, path.LastIndexOf('\\'));
        }
    }
}
