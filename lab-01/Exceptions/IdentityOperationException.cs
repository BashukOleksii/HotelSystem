namespace lab_01.Exceptions
{
    public class IdentityOperationException : Exception
    {
        public IdentityOperationException(string message)
            : base(message)
        {
        }

        public IdentityOperationException(
            IEnumerable<string> errors)
            : base(string.Join(" ", errors))
        {
        }
    }
}