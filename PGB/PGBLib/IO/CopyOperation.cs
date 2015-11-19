using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
        }
    }
}
