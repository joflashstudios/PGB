﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PGBLib.IO.Win32;

namespace PGBLib.IO
{
    public class MoveOperation : CopyOperation
    {
        public override void DoOperation()
        {
            DoOperation(null);
        }

        /// <summary>
        /// Whether to delete the folder after deleting the last file out of it on a move operation
        /// </summary>
        public bool DeleteEmptyFolder { get; set; }

        public override void DoOperation(CopyProgressCallback callback)
        {
            bool cancel = false;
            DoOperation(callback, ref cancel);
        }

        public override void DoOperation(CopyProgressCallback callback, ref bool cancel)
        {
            //Moves on the same volume should be so fast we can treat them as deletes or renames,
            //no progress needed. On other volumes, just fake the move and use the copy system.
            if (Path.GetPathRoot(Path.GetFullPath(File)) ==
                Path.GetPathRoot(Path.GetFullPath(TransferDestination)))
            {
                PrepareDirectory();
                System.IO.File.Move(File, TransferDestination);
            }
            else
            {
                base.DoOperation(callback, ref cancel);
                System.IO.File.Delete(File);
            }

            if (DeleteEmptyFolder && Directory.GetFileSystemEntries(Path.GetDirectoryName(File)).Length == 0)
            {                
                Directory.Delete(Path.GetDirectoryName(File));
            }
        }
    }
}
