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

        /// <summary>
        /// Provides the number of bytes that must be physically modified for this operation.
        /// For instance, renames and deletes are always 0, copies are always the file size,
        /// and moves will be either 0 or the file size depending on if they switch volumes.
        /// </summary>
        public abstract long EffectiveFileSize { get; }

        public abstract void DoOperation();
    }
}
