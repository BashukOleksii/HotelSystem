namespace lab_01.Exceptions
{
    public class BookingConflictException
        : ConflictException
    {
        public BookingConflictException()
            : base("Кімната вже заброньована на вибраний період.")
        {
        }

        public BookingConflictException(string message)
            : base(message)
        {
        }
    }
}