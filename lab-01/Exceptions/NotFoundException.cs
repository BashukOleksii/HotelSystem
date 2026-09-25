namespace lab_01.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(
            string entityName,
            string id)
            : base($"{entityName} з ідентифікатором '{id}' не знайдено.")
        {
        }
    }
}