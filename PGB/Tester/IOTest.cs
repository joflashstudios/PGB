using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PGBLib.IO;

namespace PGBLib.IO
{
    public static class IOTest
    {
        static OperationManager manager = new OperationManager(5);
        static Dictionary<string, string> filesInProgress = new Dictionary<string, string>();

        public static void RunTest()
        {
            manager.ProgressMade += Manager_ProgressMade;

            int i = -1;
            foreach (string s in Directory.GetFiles(@"D:\TestD", "*", SearchOption.AllDirectories))
            {
                IOOperation op;
                i++;
                IOOperation op;
                if (i == 0)
                {
                    DeleteOperation del = new DeleteOperation();
                    del.DeleteEmptyFolder = true;
                    op = del;
                }
                else if (i == 1)
                {
                    RenameOperation ren = new RenameOperation();
                    ren.NewFileName = Path.GetFileName(s) + "newfilename";
                    op = ren;
                }
                else if (i == 2)
                {
                    CopyOperation cop = new CopyOperation();
                    cop.TransferDestination = s + "copiedfile";
                    op = cop;
                }
                else
                {
                    MoveOperation mop = new MoveOperation();
                    mop.TransferDestination = s + "movedfile";
                    op = mop;
                    i = 0;
                }
                op.FileName = s;
                manager.AddOperation(op);
            }
            manager.Start();
        }

        private static object consoleLock = new object();

        private static void Manager_ProgressMade(object sender, OperationProgressDetails progress)
        {
            string percentOps = (((double)manager.OperationsProcessed) / (manager.OperationsProcessed + manager.OperationsPending) * 100).ToString("0.00");
            string percentBytes = (((double)manager.BytesProcessed) / (manager.BytesProcessed + manager.BytesPending) * 100).ToString("0.00");

            lock (consoleLock)
            {
                if (progress.ProgressType == OperationProgressType.Completed)
                {
                    if (filesInProgress.ContainsKey(progress.Operation.FileName))
                    {
                        filesInProgress.Remove(progress.Operation.FileName);
                    }
                }

                if (progress.ProgressType == OperationProgressType.InProgress)
                {
                    filesInProgress[progress.Operation.FileName] = SizeSuffix(progress.BytesTransferred) + " / " + SizeSuffix(progress.BytesTotal) + " " + progress.PercentComplete + "%";
                }

                Console.Clear();
                Console.WriteLine("Operations Complete: {0} | Operations Pending: {1}", manager.OperationsProcessed, manager.OperationsPending);
                Console.WriteLine("Bytes Complete: {0} | Bytes Pending: {1}", SizeSuffix(manager.BytesProcessed), SizeSuffix(manager.BytesPending));
                Console.WriteLine("{0}% Complete By Operations | {1}% Complete By Files", percentOps, percentBytes);
                Console.WriteLine("--File Progress--");

                foreach (KeyValuePair<string, string> kvp in filesInProgress)
                {
                    Console.WriteLine(kvp.Key);
                    Console.WriteLine(kvp.Value);
                    Console.WriteLine();
                }
            }
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
