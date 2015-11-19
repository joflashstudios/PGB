using System;

namespace PGBLib.IO.Win32
{
    // Delegates
    /// <summary>
    /// Used internally to talk to Kernal32.dll
    /// </summary>
    delegate CopyProgressResult CopyProgressRoutine(Int64 TotalFileSize, Int64 TotalBytesTransferred, Int64 StreamSize, Int64 StreamBytesTransferred, UInt32 dwStreamNumber, CopyProgressCallbackReason dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData);

    /// <summary>
    /// Used to allow clients to monitor file copies
    /// </summary>
    /// <param name="TotalFileSize">The total size of the file to be copied, in bytes</param>
    /// <param name="TotalBytesTransferred">The number of bytes that have already been transferred</param>
    /// <param name="callbackReason">The reason the callback was triggered</param>
    /// <param name="sourceFile">The name of the file being copied</param>
    /// <param name="destinationFile">The name of the destination file</param>
    /// <returns></returns>
    public delegate CopyProgressResult CopyProgressCallback(Int64 TotalFileSize, Int64 TotalBytesTransferred, string sourceFile, string destinationFile);
}
