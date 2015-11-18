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
using System.ComponentModel;
using System.IO;

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
        internal enum Win32Error : uint
        {
            ERROR_FILE_NOT_FOUND = 0x2,
            ERROR_PATH_NOT_FOUND = 0x3,
            ERROR_ACCESS_DENIED = 0x5,
            ERROR_INVALID_DRIVE = 0xF,
            //ERROR_WRITE_PROTECT = 0x13,
            ERROR_SHARING_VIOLATION = 0x20
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
                    bool result = CopyFileEx(source, destination, progressHandler, IntPtr.Zero, cancelp, flags);
                    if (!result)
                    {
                        HandleCopyExError(source, destination);
                        return;
                    }
                }
            }
        }

        private static void HandleCopyExError(string source, string destination)
        {
            Win32Exception win32Exception = new Win32Exception(Marshal.GetLastWin32Error());
            Exception error;
            switch ((Win32Error)win32Exception.NativeErrorCode)
            {
                case Win32Error.ERROR_ACCESS_DENIED:
                    error = new UnauthorizedAccessException(
                        string.Format("Access was denied to copy '{0}' to '{1}'.", source, destination),
                        win32Exception);
                    break;
                case Win32Error.ERROR_FILE_NOT_FOUND | Win32Error.ERROR_PATH_NOT_FOUND:
                    error = new FileNotFoundException(
                        string.Format("The file '{0}' could not be found.", source),
                        source,
                        win32Exception);
                    break;
                case Win32Error.ERROR_INVALID_DRIVE:
                    error = new DriveNotFoundException(
                        string.Format("The source or destination drive was not found when copying '{0}' to '{1}'.", source, destination),
                        win32Exception);
                    break;
                case Win32Error.ERROR_SHARING_VIOLATION:
                    error = new SharingViolationException(
                        string.Format("The source or destination file was in use when copying '{0}' to '{1}'.", source, destination),
                        win32Exception);
                    break;
                default:
                    error = win32Exception;
                    break;
            }
            throw error;
        }
    }
}
