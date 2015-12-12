using System;

namespace PGBLib.IO.Win32
{
    // Delegates
    /// <summary>
    /// Used internally to talk to Kernal32.dll
    /// </summary>
    delegate IOProgressResult CopyProgressRoutine(Int64 TotalFileSize, Int64 TotalBytesTransferred, Int64 StreamSize, Int64 StreamBytesTransferred, UInt32 dwStreamNumber, CopyProgressCallbackReason dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData);

    /// <summary>
    /// Used to allow clients to monitor file copies
    /// </summary>
    /// <param name="TotalBytes">The total size of the file to be copied, in bytes</param>
    /// <param name="ProcessedBytes">The number of bytes that have already been processed</param>
    /// <param name="callbackReason">The reason the callback was triggered</param>
    /// <param name="sourceFile">The name of the file being copied</param>
    /// <param name="destinationFile">The name of the destination file</param>
    /// <returns></returns>
    public delegate IOProgressResult IOProgressCallback(Int64 TotalBytes, Int64 ProcessedBytes, string sourceFile, string destinationFile);
}
