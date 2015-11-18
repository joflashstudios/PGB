namespace System.IO
{
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
