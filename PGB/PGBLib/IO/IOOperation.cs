using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PGBLib.IO
{
    /// <summary>
    /// Represents an IOOperation, which can be fed to an OperationWorker or OperationManager.
    /// Note: these are ALL synchronus, blocking methods. Some of them have callbacks, but they don't return until they've finished or thrown an exception.
    /// Regardless of 100% callbacks, etc, they should be considered incomplete until they return.
    /// </summary>
    public abstract class IOOperation
    {
        /// <summary>
        /// The file to operate on
        /// </summary>
        public string FileName { get; set; }

        public long FileSize
        {
            get
            {
                if (_FileSize == -1)
                    _FileSize = new FileInfo(FileName).Length;
                return _FileSize;
            }
        }

        private long _FileSize = -1;

        public abstract void DoOperation();
    }
}
