namespace System.IO
{
    [Serializable]
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
