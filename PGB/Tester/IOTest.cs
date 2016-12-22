using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PGBLib.Core;

namespace PGBLib.IO.Win32
{
    public static class IOTest
    {
        public static void RunTest()
        {
            PurgeBackup backup = new PurgeBackup();

            backup.Source = @"C:\Users\Trinity\Documents\Downloads";
            backup.Destination = @"H:\Backups\Download Purge";

            backup.BackupProgress += Task_BackupProgress;
            backup.Complete += Task_Complete;

            backup.StaleTime = TimeSpan.FromDays(30);
            backup.ExpiredTime = TimeSpan.FromDays(365);

            backup.Run();

            backup.SaveToFile("D:\\task.pgb");
            Console.ReadKey();
        }

        private static void Task_Complete()
        {
            Console.WriteLine("Done!");
        }

        private static void Task_BackupProgress(object sender, BackupProgressEventArgs e)
        {
            Console.WriteLine(e.PercentComplete + "%");
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
