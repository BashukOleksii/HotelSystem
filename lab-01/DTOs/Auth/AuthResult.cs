namespace lab_01.DTOs.Auth
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }

        public IReadOnlyList<string> Errors { get; set; }
            = [];

        public static AuthResult Success()
        {
            return new AuthResult
            {
                Succeeded = true
            };
        }

        public static AuthResult Failure(
            params string[] errors)
        {
            return new AuthResult
            {
                Succeeded = false,
                Errors = errors
            };
        }
    }
}