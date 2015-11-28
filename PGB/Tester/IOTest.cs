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
                DeleteOperation op = new DeleteOperation();
                op.FileName = s;
                op.DeleteEmptyFolder = true;
                op.DoOperation();
            }
        }
    }
}
