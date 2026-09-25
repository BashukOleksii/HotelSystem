using lab_01.DTOs.Base;

namespace lab_01.DTOs.User
{
    public class UserReadDto : BaseReadDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
