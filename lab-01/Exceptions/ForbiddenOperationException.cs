namespace lab_01.Exceptions
{
    public class ForbiddenOperationException : Exception
    {
        public ForbiddenOperationException(string message)
            : base(message)
        {
        }
    }
}