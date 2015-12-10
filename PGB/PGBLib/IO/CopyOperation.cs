using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGBLib.IO.Win32;
using System.IO;

namespace PGBLib.IO
{
    public class CopyOperation : IOOperation
    {
        /// <summary>
        /// Whether to overwrite or throw an exception if the file already exists
        /// </summary>
        public bool Overwrite { get; set; }

        /// <summary>
        /// Whether to create the destination folder or throw an exception if the folder does not exist
        /// </summary>        
        public bool CreateFolder { get; set; }

        public override long EffectiveFileSize
        {
            get
            {
                if (effectiveFileSize == -1)
                    effectiveFileSize = new FileInfo(FileName).Length;
                return effectiveFileSize;
            }
        }

        protected long effectiveFileSize = -1;

        /// <summary>
        /// The destination (if any) to move/copy the file to
        /// </summary>
        public string TransferDestination { get; set; }

        public override void DoOperation()
        {
            DoOperation(null);
        }
        
        /// <param name="callback">The callback routine to call on progress</param>
        public virtual void DoOperation(CopyProgressCallback callback)
        {
            bool cancel = false;
            DoOperation(callback, ref cancel);
        }

        /// <param name="callback">The callback routine to call on progress</param>
        public virtual void DoOperation(CopyProgressCallback callback, ref bool cancel)
        {
            PrepareDirectory();

            CopyFileFlags flags = 0;
            if (!this.Overwrite)
                flags = flags | CopyFileFlags.COPY_FILE_FAIL_IF_EXISTS;

            FileCopier.Copy(Path.GetFullPath(FileName), Path.GetFullPath(TransferDestination), ref cancel, callback, flags);
        }

        protected void PrepareDirectory()
        {
            string transferDirectory = Path.GetDirectoryName(TransferDestination);
            if (!Directory.Exists(transferDirectory))
            {
                if (CreateFolder)
                {
                    CreateDirectoryTree(Path.GetDirectoryName(FileName), transferDirectory);
                }
                else
                {
                    throw new DirectoryNotFoundException(string.Format("The directory '{0}' could not be found.", transferDirectory));
                }
            }
        }

        private void CreateDirectoryTree(string bottomTemplateDirectory, string bottomDirectory)
        {
            //For when we don't have a matching source structure:
            if (string.IsNullOrEmpty(bottomTemplateDirectory))
            {
                //Creating the parent directory if needed
                if (!Directory.Exists(GetParent(bottomDirectory)))
                {
                    CreateDirectoryTree(null, GetParent(bottomDirectory));
                }
                //Creating the current directory
                Directory.CreateDirectory(bottomDirectory);
                return;
            }
            
            //For when we do have a matching source structure:
            //Creating the parent directory if needed
            if (!Directory.Exists(GetParent(bottomDirectory)))
            {
                if (Path.GetFileName(GetParent(bottomTemplateDirectory)) == Path.GetFileName(GetParent(bottomDirectory))) //do the parent names match?
                {
                    CreateDirectoryTree(GetParent(bottomTemplateDirectory), GetParent(bottomDirectory));
                }
                else //Switch off cloning for the remainder of the directory tree - we ran into something that didn't match.
                {
                    CreateDirectoryTree(null, GetParent(bottomDirectory));
                }                
            }
            //Creating the current directory
            if (Path.GetFileName(bottomTemplateDirectory) == Path.GetDirectoryName(bottomDirectory))
            {
                DirectoryCloner.CloneDirectory(bottomTemplateDirectory, bottomDirectory);
            }
            else
            {
                Directory.CreateDirectory(bottomDirectory);
            }
        }

        //NOTE: I'm concerned this isn't robust enough.
        private string GetParent(string path)
        {
            return path.Substring(0, path.LastIndexOf('\\'));
        }
    }
}
