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

        public OperationState State { get { return _State; } }

        private OperationState _State;

        public OperationManager()
        {
            _Workers = new Dictionary<RootSet, OperationWorker>();
            _State = OperationState.Initialized;
        }

        /// <summary>
        /// Start this OperationManager, or resume from a paused state.
        /// </summary>
        public void Start()
        {
            if (State == OperationState.Terminated)
                throw new InvalidOperationException("This OperationManager has been terminated.");
            
            foreach (KeyValuePair<RootSet, OperationWorker> pair in _Workers)
            {
                pair.Value.Start();
            }            
        }

        public void AddOperation(IOOperation op)
        {
            if (State == OperationState.Terminated)
                throw new InvalidOperationException("This OperationManager has been terminated.");

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

            if (State == OperationState.Running)
            {
                worker.Start();
            }
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

        public void Terminate()
        {
            _State = OperationState.Terminated;
            foreach(KeyValuePair<RootSet, OperationWorker> set in _Workers)
            {
                set.Value.Terminate();
            }
        }

        public void Pause()
        {
            _State = OperationState.Paused;
            foreach (KeyValuePair<RootSet, OperationWorker> set in _Workers)
            {
                set.Value.Pause();
            }
        }
    }
}
