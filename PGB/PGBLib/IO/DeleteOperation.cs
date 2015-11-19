using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGBLib.IO
{
    class DeleteOperation : IOOperation
    {
        /// <summary>
        /// Whether to delete the folder after deleting the last file out of it on a delete operation
        /// </summary>
        public bool DeleteEmptyFolder { get; set; }

        public override void DoOperation()
        {
            throw new NotImplementedException();
        }
    }
}
