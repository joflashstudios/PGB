using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGBLib.IO.Win32;

namespace PGBLib.IO
{
    /// <summary>
    /// Represents an IOOperation that is not one-off, but has trackable progress.
    /// </summary>
    public abstract class OngoingOperation : IOOperation
    {
        public abstract void DoOperation(IOProgressCallback callback);
    }
}
