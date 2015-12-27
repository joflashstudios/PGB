using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGBLib.IO
{
    public static class Helpers
    {
        public static string TransformPath(string source, string destination, string path)
        {
            if (!source.EndsWith("\\"))
                source += "\\";
            if (!destination.EndsWith("\\"))
                destination += "\\";

            return destination + path.Remove(0, source.Length);
        }
    }
}
