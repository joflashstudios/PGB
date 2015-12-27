/*
Includes code from CodeProject: http://www.codeproject.com/Articles/36647/How-to-copy-files-in-C-with-a-customizable-progres
Licensed under the CodeProject Open License, which you can find here: http://www.codeproject.com/info/cpol10.aspx
*/

using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;

namespace PGBLib.IO.Win32
{
    /// <summary>
    /// Copies individual files with progress callbacks
    /// </summary>
    static class FileCopier
    {
        // Kernal32 Calls
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern unsafe bool CopyFileEx(string lpExistingFileName, string lpNewFileName, CopyProgressRoutine lpProgressRoutine, IntPtr lpData, Boolean* pbCancel, CopyFileFlags dwCopyFlags);

        /// <summary>
        /// Copies the specified file to the specified destination path
        /// </summary>
        /// <param name="source">The file to copy</param>
        /// <param name="destination">The full path and file name to copy the file to</param>
        /// <param name="cancel">A reference to a cancelation flag. If set to false during operation, the copy will cancel.</param>
        internal static void Copy(string source, string destination)
        {
            Copy(source, destination, 0);
        }

        /// <summary>
        /// Copies the specified file to the specified destination path
        /// </summary>
        /// <param name="source">The file to copy</param>
        /// <param name="destination">The full path and file name to copy the file to</param>
        /// <param name="cancel">A reference to a cancelation flag. If set to false during operation, the copy will cancel.</param>
        /// <param name="progressHandler">A <see cref="IOProgressCallback"/> to report progress to</param>
        internal static void Copy(string source, string destination, CopyFileFlags flags)
        {
            Copy(source, destination, flags, null);
        }

        internal static void Copy(string source, string destination, CopyFileFlags flags, IOProgressCallback progressHandler)
        {
            bool dummy = false; //Cancel is handled exclusively via progressHandler
            Copy(source, destination, flags, progressHandler, ref dummy);
        }

        /// <summary>
        /// Copies the specified file to the specified destination path
        /// </summary>
        /// <param name="source">The file to copy</param>
        /// <param name="destination">The path and file name to copy the file to</param>
        /// <param name="progressHandler">A <see cref="IOProgressCallback"/> to report progress to</param>
        /// <param name="flags">A <see cref="CopyFileFlags"/> indicating copy options</param>
        private static void Copy(string source, string destination, CopyFileFlags flags, IOProgressCallback progressHandler, ref bool cancel)
        {
            source = Path.GetFullPath(source); destination = Path.GetFullPath(destination);
            unsafe
            {
                fixed (Boolean* cancelp = &cancel)
                {
                    CopyProgressRoutine routine = (total, transferred, streamSize, StreamByteTrans, dwStreamNumber, reason, hSourceFile, hDestinationFile, lpData) => {
                        if (progressHandler != null)
                        {
                            return progressHandler(total - transferred, transferred, source, destination);
                        }
                        return IOProgressResult.PROGRESS_CONTINUE;
                    };
                    
                    if (!CopyFileEx(source, destination, routine, IntPtr.Zero, cancelp, flags))
                    {
                        HandleCopyExError(source, destination, Marshal.GetLastWin32Error());
                    }
                }
            }
        }

        /// <summary>
        /// Handles any errors returned from CopyEx
        /// </summary>
        private static void HandleCopyExError(string source, string destination, int errorCode)
        {
            Win32Exception win32Exception = new Win32Exception(errorCode);
            Exception error;
            switch ((Win32Error)errorCode)
            {
                case Win32Error.ACCESS_DENIED:
                    error = new UnauthorizedAccessException(
                        string.Format("Access was denied to copy '{0}' to '{1}'.", source, destination),
                        win32Exception);
                    break;
                case Win32Error.FILE_NOT_FOUND | Win32Error.PATH_NOT_FOUND:
                    error = new FileNotFoundException(
                        string.Format("The file '{0}' could not be found.", source),
                        source,
                        win32Exception);
                    break;
                case Win32Error.SHARING_VIOLATION:
                    error = new SharingViolationException(
                        string.Format("The source or destination file was in use when copying '{0}' to '{1}'.", source, destination),
                        win32Exception);
                    break;
                case Win32Error.DISK_FULL | Win32Error.HANDLE_DISK_FULL:
                    error = new DiskFullException(
                        string.Format("The destination disk for '{0}' is full.", destination),
                        win32Exception);
                    break;
                case Win32Error.REQUEST_ABORTED:
                    return;
                default:
                    error = win32Exception;
                    break;
            }
            throw error;
        }
    }
}
