using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGBLib.IO
{
    class IOOperation
    {
        /// <summary>
        /// The type of file operation
        /// </summary>
        public IOOperationType Type { get; set; }

        /// <summary>
        /// Whether to overwrite or throw an exception if the file already exists
        /// </summary>
        public bool Overwrite { get; set; }

        /// <summary>
        /// Whether to create the destination folder or throw an exception if the folder does not exist
        /// </summary>        
        public bool CreateFolder { get; set; }

        /// <summary>
        /// Whether to delete the folder after deleting the last file out of it on a delete operation
        /// </summary>
        public bool DeleteEmptyFolder { get; set; }

        /// <summary>
        /// The file to operate on
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// The destination (if any) to move/copy the file to
        /// </summary>
        public string TransferDestination { get; set; }
    }

    public enum IOOperationType
    {
        Copy,
        Move,
        Delete
    }
}
