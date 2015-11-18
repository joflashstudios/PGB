using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGBLib.IO
{
    public static class IOTest
    {
        public static void RunTest()
        {
            FileCopier.CopyProgressRoutine routine = 
                (total, transferred, streamSize, StreamByteTrans, dwStreamNumber, reason, hSourceFile, hDestinationFile, lpData) =>
            {
                return FileCopier.CopyProgressResult.PROGRESS_CONTINUE;
            };

            bool cancel = false;
            FileCopier.Copy(@"C:\Windows\Installer\781e2ab.msp", @"C:\filetest.msp", ref cancel, routine);
        }
    }
}
