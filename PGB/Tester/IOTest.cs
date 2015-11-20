using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PGBLib.IO
{
    public static class IOTest
    {
        public static void RunTest()
        {
            int i = 0;
            foreach(string s in Directory.GetFiles(@"C:\testtmp", "*", SearchOption.AllDirectories))
            {
                i++;
                MoveOperation op = new MoveOperation();
                op.File = s;
                op.CreateFolder = true;
                if (i % 2 == 0)
                {
                    op.TransferDestination = @"C:\testtmp\dumpfolder\" + Path.GetFileName(s);
                }
                else
                {
                    op.TransferDestination = @"D:\testtmp\" + Path.GetFileName(s);
                }
                op.DoOperation();
            }
        }
    }
}
