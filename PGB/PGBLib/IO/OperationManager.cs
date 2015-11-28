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

        private bool _Running;

        public OperationManager()
        {
            _Workers = new Dictionary<RootSet, OperationWorker>();
        }

        public void Start()
        {
            _Running = true;
            foreach(KeyValuePair<RootSet, OperationWorker> pair in _Workers)
            {
                pair.Value.Start();
            }
        }

        public void AddOperation(IOOperation op)
        {
            string root = Path.GetPathRoot(op.FileName);
            string destinationRoot = "";
            if (op is CopyOperation)
            {
                destinationRoot = Path.GetPathRoot(((CopyOperation)op).TransferDestination);
            }
            RootSet roots = new RootSet(root, destinationRoot);

            if (!_Workers.ContainsKey(roots))
                RegisterWorker(roots);

            _Workers[roots].EnqueueOperation(op);
        }

        private void RegisterWorker(RootSet roots)
        {
            OperationWorker worker = new OperationWorker();
            worker.ProgressMade += Worker_ProgressMade;
            _Workers.Add(roots, new OperationWorker());

            if (_Running)
                worker.Start();
        }

        private void Worker_ProgressMade(object sender, OperationProgressDetails progress)
        {
            throw new NotImplementedException();
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
