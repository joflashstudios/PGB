using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGBLib.IO.Win32
{
    // Delegates
    delegate CopyProgressResult CopyProgressRoutine(Int64 TotalFileSize, Int64 TotalBytesTransferred, Int64 StreamSize, Int64 StreamBytesTransferred, UInt32 dwStreamNumber, CopyProgressCallbackReason dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData);
    delegate CopyProgressResult CopyProgressCallback(Int64 TotalFileSize, Int64 TotalBytesTransferred, CopyProgressCallbackReason callbackReason, string sourceFile, string destinationFile);
}
