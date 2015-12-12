using System;
using System.Linq;
using System.IO;

namespace PGBLib.IO
{
    public class RenameOperation : IOOperation
    {
        public string NewFileName { get; set; }

        public RenameOperation()
        {

        }
    
        public RenameOperation(string fileName, string newFileName)
        {
            FileName = fileName;
            NewFileName = newFileName;
        }

        public override long EffectiveFileSize
        {
            get
            {
                return 0;
            }
        }

        public override void DoOperation()
        {
            if (string.IsNullOrEmpty(NewFileName) || string.IsNullOrEmpty(FileName))
                throw new InvalidOperationException("Both NewFileName and FileName must be specified.");

            if (NewFileName.Contains('\\'))
            {
                if (Path.GetDirectoryName(FileName).ToLower() != Path.GetDirectoryName(NewFileName.ToLower()))
                    throw new InvalidOperationException("Can only rename files to the same folder. To move a file, use MoveOperation.");
                else
                    File.Move(FileName, NewFileName);
            }
            else
            {
                File.Move(FileName, Path.Combine(Path.GetDirectoryName(FileName), NewFileName));
            }
        }

        public override string ToString()
        {
            return "Rename " + FileName + " to " + NewFileName;
        }
    }
}
