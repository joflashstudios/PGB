﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PGBLib.IO
{
    public class DeleteOperation : IOOperation
    {
        /// <summary>
        /// Whether to delete the folder after deleting the last file out of it on a delete operation
        /// </summary>
        public bool DeleteEmptyFolder { get; set; }

        public override void DoOperation()
        {
            System.IO.File.Delete(File);
            string directory = Path.GetDirectoryName(File);
            if (DeleteEmptyFolder && Directory.GetFileSystemEntries(directory).Length == 0)
            {
                Directory.Delete(directory);
            }
        }
    }
}
