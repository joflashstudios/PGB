namespace System.IO
{
    /// <summary>
    /// Represents an exception thrown when there is a sharing violation trying to access a file
    /// </summary>
    class SharingViolationException : IOException
    {
        public SharingViolationException() : base()
        {

        }

        public SharingViolationException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
