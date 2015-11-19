namespace System.IO
{
    class DiskFullException : IOException
    {
        public DiskFullException() : base()
        {

        }

        public DiskFullException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
