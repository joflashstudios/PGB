using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Linq;

namespace PGBLib.IO
{
    public delegate void ScannerErrorHandler(FileSystemInfo obj, Exception e);

    public class DirectoryScanner : IEnumerable<FileInfo>
    {
        private string path;
        public HashSet<string> blacklist { get; set; }
        public event ScannerErrorHandler Errored;

        protected virtual void OnError(FileSystemInfo obj, Exception e)
        {
            //Raise the Tick event (see below for an explanation of this)
            var errorEvent = Errored;
            if (errorEvent != null)
                errorEvent(obj, e);
        }

        public event DirectoryScanErrorHandler ScannerErrored;

        public DirectoryScanner(string path)
        {
            this.path = path;
            Blacklist = new HashSet<string>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            DirectoryEnumerator en = new DirectoryEnumerator(path, blacklist);
            en.Errored += EnumeratorErrored;
            return en;            
        }

        public IEnumerator<FileInfo> GetEnumerator()
        {
            DirectoryEnumerator en = new DirectoryEnumerator(path, blacklist);
            en.Errored += EnumeratorErrored;
            return en;
        }

        private void EnumeratorErrored(FileSystemInfo obj, Exception e)
        {
            OnError(obj, e);
        }

        class DirectoryEnumerator : IEnumerator<FileInfo>
        {
            private DirectoryInfo topDirectory;
            private HashSet<string> blacklist;

            public event DirectoryScanErrorHandler ScannerErrored;

            IEnumerator<FileSystemInfo> enumerator;

            Stack<DirectoryInfo> directories;

            public event ScannerErrorHandler Errored;

            protected virtual void OnError(FileSystemInfo obj, Exception e)
            {
                //Raise the Tick event (see below for an explanation of this)
                var errorEvent = Errored;
                if (errorEvent != null)
                    errorEvent(obj, e);
            }

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
                    if (!enumerator.MoveNext())
                        return NextDirectory();
                
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
                    return NextDirectory();

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
                    try
                    {
                    enumerator = dir.EnumerateFileSystemInfos().GetEnumerator();
                    return MoveNext();
                }
                    catch (UnauthorizedAccessException e)
                    {
                        OnError(dir, e);
                        return NextDirectory();
                    }
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
