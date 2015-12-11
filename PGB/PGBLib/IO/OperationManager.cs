using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace PGBLib.IO
{
    class OperationManager
    {
        private Dictionary<RootSet, OperationWorker> workers;

        public OperationState State { get { return state; } }

        private OperationState state;

        public long BytesPending
        {
            get
            {
                return workers.Sum(n => n.Value.CopyBytesPending);
            }
        }

        public int OperationsPending
        {
            get
            {
                return workers.Sum(n => n.Value.OperationsPending);
            }
        }

        public OperationManager()
        {
            workers = new Dictionary<RootSet, OperationWorker>();
            state = OperationState.Initialized;
        }

        /// <summary>
        /// Start this OperationManager, or resume from a paused state.
        /// </summary>
        public void Start()
        {
            if (State == OperationState.Terminated)
                throw new InvalidOperationException("This OperationManager has been terminated.");
            
            foreach (KeyValuePair<RootSet, OperationWorker> pair in workers)
            {
                pair.Value.Start();
            }

            state = OperationState.Running;        
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

            if (!workers.ContainsKey(roots))
                RegisterWorker(roots);

            workers[roots].EnqueueOperation(op);
        }

        private void RegisterWorker(RootSet roots)
        {
            OperationWorker worker = new OperationWorker();
            worker.ProgressMade += Worker_ProgressMade;
            workers.Add(roots, new OperationWorker());

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
            foreach(KeyValuePair<RootSet, OperationWorker> set in workers)
            {
                set.Value.Terminate();
            }
            state = OperationState.Terminated;
        }

        public void Pause()
        {
            foreach (KeyValuePair<RootSet, OperationWorker> set in workers)
            {
                set.Value.Pause();
            }
            state = OperationState.Paused;
        }
    }
}
