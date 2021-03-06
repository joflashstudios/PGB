﻿using System;

namespace PGBLib.IO.Win32
{
    /// <summary>
    /// Represents options for file copy operations
    /// </summary>
    [Flags]
    internal enum CopyFileFlags : uint
    {
        COPY_FILE_FAIL_IF_EXISTS = 0x00000001,
        COPY_FILE_RESTARTABLE = 0x00000002,
        COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,
        COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008
    }

    /// <summary>
    /// Returned from file copy progress callbacks to tell windows what to do next
    /// </summary>
    public enum IOProgressResult : uint
    {
        PROGRESS_CONTINUE = 0,
        PROGRESS_CANCEL = 1,
        PROGRESS_STOP = 2,
        PROGRESS_QUIET = 3
    }

    /// <summary>
    /// Indicates the reason the file copy progress callback was called
    /// </summary>
    internal enum CopyProgressCallbackReason : uint
    {
        CALLBACK_CHUNK_FINISHED = 0x00000000,
        CALLBACK_STREAM_SWITCH = 0x00000001
    }

    /// <summary>
    /// Enumeration of Win32 error codes relevent to PGBLib.IO.Win32
    /// </summary>
    internal enum Win32Error : uint
    {
        FILE_NOT_FOUND = 0x2,
        PATH_NOT_FOUND = 0x3,
        ACCESS_DENIED = 0x5,
        WRITE_PROTECT = 0x13,
        SHARING_VIOLATION = 0x20,
        ALREADY_EXISTS = 0xB7,
        REQUEST_ABORTED = 0x4D3,
        DISK_FULL = 0x70,
        HANDLE_DISK_FULL = 0x27
    }
}
