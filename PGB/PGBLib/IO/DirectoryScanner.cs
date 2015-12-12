using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PGBLib.IO
{
    public class DirectoryScanner : IEnumerable<FileInfo>
    {
        private string path;

        public DirectoryScanner(string path)
        {
            this.path = path;
        }

        public IEnumerator<FileInfo> GetEnumerator()
        {
            return new DirectoryEnumerator(path);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DirectoryEnumerator(path);
        }

        class DirectoryEnumerator : IEnumerator<FileInfo>
        {
            private DirectoryInfo topDirectory;

            IEnumerator<FileSystemInfo> enumerator;

            Stack<DirectoryInfo> directories;

            public DirectoryEnumerator(string path)
            {
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

            public bool MoveNext()
            {
                if (enumerator.MoveNext())
                {
                    bool continuing = true;
                    while (enumerator.Current is DirectoryInfo)
                    {
                        directories.Push((DirectoryInfo)enumerator.Current);
                        continuing = enumerator.MoveNext();
                    }

                    if (continuing)
                    {
                        current = (FileInfo)enumerator.Current;
                        return true;
                    }
                    else
                    {
                        return DrillDown();
                    }
                }
                else
                {
                    return DrillDown();
                }
            }

            private bool DrillDown()
            {
                if (directories.Count > 0)
                {
                    DirectoryInfo dir = directories.Pop();
                    enumerator.Dispose();
                    enumerator = dir.EnumerateFileSystemInfos().GetEnumerator();
                    return MoveNext();
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                enumerator.Reset();
            }
        }
    }
}
