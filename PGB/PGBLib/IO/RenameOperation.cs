using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PGBLib.IO
{
    public class RenameOperation : IOOperation
    {
        /// <summary>
        /// The new file name to use. Should not include directory.
        /// </summary>
        public string NewFileName { get; set; }

        public override void DoOperation()
        {
            //TODO: not sure this is robust enough.
            if (NewFileName.Contains('\\'))
            {
                throw new InvalidOperationException("NewFileName should not include directory information.");
            }
            System.IO.File.Move(File, Path.Combine(Path.GetDirectoryName(File), NewFileName));
        }
    }
}
