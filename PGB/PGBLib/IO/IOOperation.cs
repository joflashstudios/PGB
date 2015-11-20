using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string File { get; set; }

        public abstract void DoOperation();
    }
}
