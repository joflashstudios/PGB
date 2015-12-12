using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGBLib.IO.Win32;
using System.IO;

namespace PGBLib.IO
{
    public class CopyOperation : OngoingOperation
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
                {
                    if (File.Exists(FileName))
                        effectiveFileSize = new FileInfo(FileName).Length;
                    else
                        return 0; //The file doesn't exist, return 0, and check again next time the property is accessed for the real value.
                }
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
        public override void DoOperation(IOProgressCallback callback)
        {
            PrepareDirectory();

            CopyFileFlags flags = CopyFileFlags.COPY_FILE_ALLOW_DECRYPTED_DESTINATION;
            if (!this.Overwrite)
                flags = flags | CopyFileFlags.COPY_FILE_FAIL_IF_EXISTS;

            FileCopier.Copy(Path.GetFullPath(FileName), Path.GetFullPath(TransferDestination), flags, callback);
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
