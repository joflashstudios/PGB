/*
Includes code from CodeProject: http://www.codeproject.com/Articles/36647/How-to-copy-files-in-C-with-a-customizable-progres
Licensed under the CodeProject Open License, which you can find here: http://www.codeproject.com/info/cpol10.aspx
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PGBLib.IO
{
    class FileCopier
    {
        // Enums
        // These Enums are used for the windows CopyFileEx function
        [Flags]
        internal enum CopyFileFlags : uint
        {
            COPY_FILE_FAIL_IF_EXISTS = 0x00000001,
            COPY_FILE_RESTARTABLE = 0x00000002,
            COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,
            COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008
        }
        internal enum CopyProgressResult : uint
        {
            PROGRESS_CONTINUE = 0,
            PROGRESS_CANCEL = 1,
            PROGRESS_STOP = 2,
            PROGRESS_QUIET = 3
        }
        internal enum CopyProgressCallbackReason : uint
        {
            CALLBACK_CHUNK_FINISHED = 0x00000000,
            CALLBACK_STREAM_SWITCH = 0x00000001
        }
      
        // Delegates
        internal delegate CopyProgressResult CopyProgressRoutine(Int64 TotalFileSize, Int64 TotalBytesTransferred, Int64 StreamSize, Int64 StreamBytesTransferred, UInt32 dwStreamNumber, CopyProgressCallbackReason dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData);

        // Kernal32 Calls
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern unsafe bool CopyFileEx(string lpExistingFileName, string lpNewFileName, CopyProgressRoutine lpProgressRoutine, IntPtr lpData, Boolean* pbCancel, CopyFileFlags dwCopyFlags);

        private static CopyProgressResult CopyProgressHandler(Int64 total, Int64 transferred, Int64 streamSize, Int64 StreamByteTrans, UInt32 dwStreamNumber, CopyProgressCallbackReason reason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData)
        {            
            //TODO: progress logic goes here
            return CopyProgressResult.PROGRESS_CONTINUE;
        }

        internal static void Copy(string source, string destination)
        {
            bool dummycancel = false;
            Copy(source, destination, ref dummycancel);
        }

        internal static void Copy(string source, string destination, ref bool cancel)
        {
            Copy(source, destination, ref cancel, new CopyProgressRoutine(CopyProgressHandler));
        }

        internal static void Copy(string source, string destination, ref bool cancel, CopyProgressRoutine progressHandler)
        {
            Copy(source, destination, ref cancel, progressHandler, 0);
        }

        internal static void Copy(string source, string destination, ref bool cancel, CopyProgressRoutine progressHandler, CopyFileFlags flags)
        {
            unsafe
            {
                fixed (Boolean* cancelp = &cancel)
                {
                    CopyFileEx(source, destination, progressHandler, IntPtr.Zero, cancelp, flags);
                }
            }
        }        
    }
}
