namespace System.IO
{
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
