using System;
using System.Linq;
using System.IO;

namespace PGBLib.IO
{
    public class RenameOperation : IOOperation
    {
        /// <summary>
        /// The new file name to use. Should not include directory.
        /// </summary>
        public string NewFileName { get; set; }

        public override long EffectiveFileSize
        {
            get
            {
                return 0;
            }
        }

        public override void DoOperation()
        {
            //TODO: not sure this is robust enough.
            if (NewFileName.Contains('\\'))
            {
                throw new InvalidOperationException("NewFileName should not include directory information.");
            }
            System.IO.File.Move(FileName, Path.Combine(Path.GetDirectoryName(FileName), NewFileName));
        }
    }
}
