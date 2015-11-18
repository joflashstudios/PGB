using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGBLib.IO.Win32
{
    public static class IOTest
    {
        public static void RunTest()
        {
            int callbackCount = 0;
            CopyProgressCallback routine = (total, transferred, reason, sourceFile, destinationFile) =>
            {
                Console.Clear();
                Console.WriteLine(Math.Round((double)transferred / (double)total, 2) * 100 + "% complete copying " + sourceFile + " to " + destinationFile + " callback # " + callbackCount);
                callbackCount++;
                return CopyProgressResult.PROGRESS_CONTINUE;
            };

            bool cancel = false;
            FileCopier.Copy(@"C:\Users\Elizabeth\Documents\TTC 15 Niger Thank You Video\Maradi Youth Camp 015.MOV", @"C:\users\elizabeth\testfile.file", ref cancel, routine);
        }
    }
}
