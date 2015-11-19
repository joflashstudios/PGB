using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGBLib.IO
{
    abstract class IOOperation
    {
        /// <summary>
        /// The file to operate on
        /// </summary>
        public string File { get; set; }

        public abstract void DoOperation();
    }
}
