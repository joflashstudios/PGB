using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGBLib.Core
{
    public static class FileInfoExtensions
    {
        public static bool IsUntouched(this FileInfo file, TimeSpan duration)
        {
            TimeSpan timeSinceTouched = new[]
            {
                file.CreationTimeUtc, file.LastAccessTimeUtc, file.LastWriteTimeUtc
            }.Max(x => DateTime.UtcNow.Subtract(x));

            return timeSinceTouched > duration;
        }

        public static string TranslatePath(this FileInfo file, string source, string destination)
        {
            if (!source.EndsWith("\\"))
                source += "\\";
            if (!destination.EndsWith("\\"))
                destination += "\\";

            return destination + file.FullName.Remove(0, source.Length);
        }
    }
}
