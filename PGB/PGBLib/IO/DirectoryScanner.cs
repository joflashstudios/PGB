using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Linq;

namespace PGBLib.IO
{
    public class DirectoryScanner : IEnumerable<FileInfo>
    {
        private string path;
        public HashSet<string> blacklist { get; set; }

        public event DirectoryScanErrorHandler ScannerErrored;

        public DirectoryScanner(string path)
        {
            this.path = path;
            blacklist = new HashSet<string>();
        }

        public IEnumerator<FileInfo> GetEnumerator()
        {
            DirectoryEnumerator enumerator = new DirectoryEnumerator(path, blacklist);
            enumerator.ScannerErrored += Enumerator_ScannerErrored;
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            DirectoryEnumerator enumerator = new DirectoryEnumerator(path, blacklist);
            enumerator.ScannerErrored += Enumerator_ScannerErrored;
            return enumerator;
        }

        private void Enumerator_ScannerErrored(UnauthorizedAccessException error)
        {
            if (ScannerErrored != null)
                ScannerErrored(error);
        }

        class DirectoryEnumerator : IEnumerator<FileInfo>
        {
            private DirectoryInfo topDirectory;
            private HashSet<string> blacklist;

            public event DirectoryScanErrorHandler ScannerErrored;

            IEnumerator<FileSystemInfo> enumerator;

            Stack<DirectoryInfo> directories;

            public DirectoryEnumerator(string path, HashSet<string> blacklist)
            {
                this.blacklist = blacklist;
                topDirectory = new DirectoryInfo(path);
                enumerator = topDirectory.EnumerateFileSystemInfos().GetEnumerator();
                directories = new Stack<DirectoryInfo>();
            }

            public FileInfo Current
            {
                get
                {
                    return current;
                }
            }

            private FileInfo current;       

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
                enumerator.Dispose();                
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                //If there's nothing left in the folder, move on
                try
                {
                    if (!enumerator.MoveNext())
                    {
                        return NextDirectory();
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    ScannerErrored(e);
                    return NextDirectory();
                }
                
                //Add subfolders to the stack until we hit a file or run out
                bool continuing = true;
                for (DirectoryInfo currentAsDirectory = this.enumerator.Current as DirectoryInfo; currentAsDirectory != null; currentAsDirectory = this.enumerator.Current as DirectoryInfo)
                {
                    if (blacklist.Count == 0 || !blacklist.Contains(currentAsDirectory.FullName))
                    {
                        directories.Push(currentAsDirectory);
                    }

                    continuing = enumerator.MoveNext();
                }

                //If we've run out, then move on
                if (!continuing)
                {
                    return NextDirectory();
                }

                //Otherwise, conditionally add the file
                if (blacklist.Count == 0 || !blacklist.Contains(enumerator.Current.FullName))
                {
                    current = (FileInfo)enumerator.Current;
                    return true;
                }

                //This file was blacklisted - move on
                return MoveNext();                
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool NextDirectory()
            {                
                if (directories.Count != 0)
                {
                    DirectoryInfo dir = directories.Pop();
                    enumerator.Dispose();
                    enumerator = dir.EnumerateFileSystemInfos().GetEnumerator();
                    return MoveNext();
                }
                return false;
            }

            public void Reset()
            {
                enumerator.Reset();
            }
        }

        public delegate void DirectoryScanErrorHandler(UnauthorizedAccessException error);
    }
}
