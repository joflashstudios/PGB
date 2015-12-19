using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PGBLib.IO;

namespace PGBLib.IO.Win32
{
    public static class IOTest
    {
        public static void RunTest()
        {

            for (int i = 0; i < 2; i++)
            {
                DateTime now = DateTime.Now;
                long bytes = 0;
                long files = 0;
                //foreach (var v in new FileEnumerable("C:\\Program Files (x86)", "*w*", SearchOption.AllDirectories))
                //{
                //    bytes += v.Size;
                //    files++;
                //}
                TimeSpan elapsed = DateTime.Now - now;

                //now = DateTime.Now;
                //bytes = 0;
                //files = 0;
                //foreach (var v in new DirectoryInfo("C:\\Program Files (x86)").GetFiles("*", SearchOption.AllDirectories))
                //{
                //    bytes += v.Length;
                //    files++;
                //}
                //elapsed = DateTime.Now - now;
                //Console.WriteLine(SizeSuffix(bytes) + " in " + files + " files in " + elapsed.TotalSeconds + " seconds (Non-perf)");

                //now = DateTime.Now;
                //bytes = 0;
                //files = 0;
                //foreach (var v in new DirectoryInfo("C:\\Program Files (x86)").EnumerateFiles("*", SearchOption.AllDirectories))
                //{
                //    bytes += v.Length;
                //    files++;
                //}
                //elapsed = DateTime.Now - now;

                //Console.WriteLine(SizeSuffix(bytes) + " in " + files + " files in " + elapsed.TotalSeconds + " seconds (enumerate)");

                now = DateTime.Now;
                bytes = 0;
                files = 0;
                DirectoryScanner scanner = new DirectoryScanner("D:\\");
                scanner.Blacklist.Add(@"C:\Program Files (x86)\Steam");
                scanner.ScannerErrored += Scanner_ScannerErrored;
                foreach (var v in scanner)
                {
                    bytes += v.Length;
                    files++;
                }
                elapsed = DateTime.Now - now;

                Console.WriteLine(SizeSuffix(bytes) + " in " + files + " files in " + elapsed.TotalSeconds + " seconds (custom enumerate)");
                Console.WriteLine();
            }

            //Console.ReadKey();
        }

        private static void Scanner_ScannerErrored(UnauthorizedAccessException error)
        {

        }

        static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(long value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }


    }
}
