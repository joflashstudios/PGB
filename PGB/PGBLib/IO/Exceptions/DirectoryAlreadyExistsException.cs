namespace System.IO
{
    /// <summary>
    /// Represents an exception to be thrown when trying to create a directory that already exists.
    /// </summary>
    [Serializable]
    class DirectoryAlreadyExistsException : IOException
    {
        public DirectoryAlreadyExistsException() : base()
        {

        }

        public DirectoryAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
