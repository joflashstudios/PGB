using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PGBLib.IO
{
    class OperationManager
    {
        private Dictionary<RootSet, OperationWorker> _Workers;

        public OperationManager()
        {
            _Workers = new Dictionary<RootSet, OperationWorker>();
        }

        public void AddOperation(IOOperation op)
        {
            string root = Path.GetPathRoot(op.File);
            string destinationRoot = "";
            if (op is CopyOperation)
            {
                destinationRoot = Path.GetPathRoot(((CopyOperation)op).TransferDestination);
            }
            RootSet roots = new RootSet(root, destinationRoot);

            if (!_Workers.ContainsKey(roots))
            {
                _Workers.Add(roots, new OperationWorker());
            }
            _Workers[roots].EnqueueOperation(op);
        }

        public void AddOperations(IEnumerable<IOOperation> ops)
        {
            foreach(IOOperation op in ops)
            {
                AddOperation(op);
            }
        }
    }
}
