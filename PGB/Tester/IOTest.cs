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
            CopyOperation op = new CopyOperation();
            op.File = @"C:\Users\Elizabeth\Documents\Niger History\Maradi Region\Atchire\Book worthy\IMG_1631.JPG";
            op.CreateFolder = true;
            op.TransferDestination = @"C:\Users\Elizabeth\Documentssss\ssssssomething\Niger History\Maradi Region\Atchire\Book worthy\IMG_1631.JPG";
            op.DoOperation();
        }
    }
}
