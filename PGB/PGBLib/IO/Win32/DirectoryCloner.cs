using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;

namespace PGBLib.IO.Win32
{
    /// <summary>
    /// Clones directories to a new location, keeping their attributes.
    /// Does NOT copy files or subdirectories.
    /// </summary>
    class DirectoryCloner
    {
        // Kernal32 Calls
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CreateDirectoryEx(string lpTemplateDirectory, string lpNewDirectory, IntPtr lpSecurityAttributes);

        /// <summary>
        /// Clones the template directory to the target location
        /// </summary>
        internal static void CloneDirectory(string template, string target)
        {
            if (!CreateDirectoryEx(template, target, IntPtr.Zero)) {
                HandleCreateDirectoryExError(template, target, Marshal.GetLastWin32Error());
            }
        }

        /// <summary>
        /// Handles any errors that CreateDirectoryEx may encounter
        /// </summary>
        private static void HandleCreateDirectoryExError(string template, string target, int errorCode)
        {
            Win32Exception win32Exception = new Win32Exception(errorCode);
            Exception error;
            switch ((Win32Error)errorCode)
            {
                case Win32Error.ACCESS_DENIED:
                    error = new UnauthorizedAccessException(
                        string.Format("Access was denied to clone '{0}' to '{1}'.", template, target),
                        win32Exception);
                    break;
                case Win32Error.PATH_NOT_FOUND:
                    error = new DirectoryNotFoundException(
                        string.Format("The path '{0}' or '{1}' could not be found.", template, target),
                        win32Exception);
                    break;
                case Win32Error.SHARING_VIOLATION:
                    error = new SharingViolationException(
                        string.Format("The source or destination file was in use when copying '{0}' to '{1}'.", template, target),
                        win32Exception);
                    break;
                case Win32Error.ALREADY_EXISTS:
                    //If the directory already exists don't worry about it.
                    return;
                default:
                    error = win32Exception;
                    break;
            }
            throw error;
        }
    }
}
