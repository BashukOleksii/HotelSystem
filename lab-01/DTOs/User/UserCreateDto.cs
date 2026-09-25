namespace lab_01.DTOs.User
{
    public class UserCreateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
